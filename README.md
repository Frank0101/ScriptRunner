# ScriptRunner

Is a little Web API that allows users to run scripts on a remote machine without needing an RDM access.
The users will be able to read the script output and its termination code, as if they started it from
a normal console.

The configuration is entirely defined through XML files, and allows to specify the scripts' properties
as long as the users' permissions.

- Is possible to give run permission for a script to an user or to an user group.
- Is possible for a script to define the delay to elapse between any execution and the next one.
- Is possible for a script to define the timeout after whose the script will be terminated forcibly.

An email will be sent to a list of recipients every time a script is executed, reporting who started
it, when, for how long the script has been running, its termination code and its output.
