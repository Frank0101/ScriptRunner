using ScriptRunner.Core.Exceptions;
using ScriptRunner.Core.Models;
using ScriptRunner.Core.Repositories;
using ScriptRunner.Core.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace ScriptRunner.Infra.Services
{
    public class ScriptRunnerService : IScriptRunnerService
    {
        private readonly IScriptsDynamicDataRepository _scriptsDynamicDataRepository;

        public ScriptRunnerService(IScriptsDynamicDataRepository scriptsDynamicDataRepository)
        {
            _scriptsDynamicDataRepository = scriptsDynamicDataRepository;
        }

        public ScriptRunResultModel RunScript(string userName, ScriptModel targetScript)
        {
            var elapsedDelayFromScriptLastRun = GetElapsedDelayFromScriptLastRun(targetScript);
            if (elapsedDelayFromScriptLastRun > targetScript.Delay)
            {
                UpdateScriptLastRun(targetScript);

                var scriptRunResult = new ScriptRunResultModel
                {
                    UserName = userName,
                    ScriptKey = targetScript.Key,
                    ScriptStartedAt = DateTime.Now,
                    ScriptOutput = new List<ScriptOutputModel>()
                };

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = targetScript.Path,
                        Arguments = targetScript.Arguments,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    },
                };

                using (var outputDataHandle = new AutoResetEvent(false))
                {
                    using (var errorDataHandle = new AutoResetEvent(false))
                    {
                        proc.OutputDataReceived += (sender, args) => OnOutputDataReceived(args, outputDataHandle, scriptRunResult);
                        proc.ErrorDataReceived += (sender, args) => OnErrorDataReceived(args, errorDataHandle, scriptRunResult);

                        proc.Start();
                        proc.BeginOutputReadLine();
                        proc.BeginErrorReadLine();
                        if (proc.WaitForExit(targetScript.Timeout * 1000) && outputDataHandle.WaitOne() && errorDataHandle.WaitOne())
                        {
                            scriptRunResult.ScriptTermination = ScriptTermination.ExecutionFinished;
                        }
                        else
                        {
                            scriptRunResult.ScriptTermination = ScriptTermination.TimeoutExpired;
                            proc.Kill();
                        }
                        scriptRunResult.ScriptExitCode = proc.ExitCode;
                        scriptRunResult.ScriptEndedAt = DateTime.Now;
                    }
                }
                return scriptRunResult;
            }
            throw new DelayNotExpiredException
            {
                RemainingDelay = targetScript.Delay - elapsedDelayFromScriptLastRun
            };
        }

        private double GetElapsedDelayFromScriptLastRun(ScriptModel targetScript)
        {
            var scriptDynamicData = _scriptsDynamicDataRepository.GetAll()
                .FirstOrDefault(script => script.Key == targetScript.Key);
            var lastRun = scriptDynamicData != null ? scriptDynamicData.LastRun : 0;
            return TimeSpan.FromTicks(DateTime.UtcNow.Ticks - lastRun).TotalSeconds;
        }

        private void UpdateScriptLastRun(ScriptModel scriptToRun)
        {
            _scriptsDynamicDataRepository.Set(new ScriptDynamicDataModel
            {
                Key = scriptToRun.Key,
                LastRun = DateTime.UtcNow.Ticks
            });
        }

        private static void OnOutputDataReceived(DataReceivedEventArgs args, AutoResetEvent dataHandle, ScriptRunResultModel scriptRunResult)
        {
            if (args.Data != null)
            {
                ((IList<ScriptOutputModel>)scriptRunResult.ScriptOutput).Add(new ScriptOutputStdModel
                {
                    Timestamp = DateTime.Now,
                    Value = args.Data
                });
            }
            else
            {
                dataHandle.Set();
            }
        }

        private static void OnErrorDataReceived(DataReceivedEventArgs args, AutoResetEvent dataHandle, ScriptRunResultModel scriptRunResult)
        {
            if (args.Data != null)
            {
                ((IList<ScriptOutputModel>)scriptRunResult.ScriptOutput).Add(new ScriptOutputErrModel
                {
                    Timestamp = DateTime.Now,
                    Value = args.Data
                });
            }
            else
            {
                dataHandle.Set();
            }
        }
    }
}
