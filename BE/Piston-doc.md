Configuration
Piston provides many different configuration options to tweak Piston to meet your needs.

Configuration is specified through environment variables, prefixed with PISTON_.

Log Level
key: PISTON_LOG_LEVEL
default: INFO
Level of log output to provide.

One of DEBUG, INFO, WARN, ERROR or NONE

Bind Address
key: PISTON_BIND_ADDRESS
default: 0.0.0.0:2000
Port and IP address to bind the Piston API to.

Warning

Changing this value is not recommended.

This changes the bind address inside the container, and thus serves no purpose when running in a container

Data Directory
key: PISTON_DATA_DIRECTORY
default: /piston
Absolute path to piston related data, including packages and job contexts.

Warning

Changing this value is not recommended.

Some packages require absolute paths on disk at build time. Due to this, some packages may break when changing this parameter.

Runner GID/UID range
key:
    - PISTON_RUNNER_UID_MIN
    - PISTON_RUNNER_UID_MAX
    - PISTON_RUNNER_GID_MIN
    - PISTON_RUNNER_GID_MAX
default:
    - 1001
    - 1500
    - 1001
    - 1500
UID and GID ranges to use when executing jobs.

Warning

Changing this value is not recommended.

The piston container creates 500 users and groups by default, and reserves user/group 1000 for running the API. Any processes run by these users will be killed when cleaning up a job.

Disable Networking
key: PISTON_DISABLE_NETWORKING
default: true
Disallows access to socket syscalls, effectively disabling networking for jobs run by piston.

Max Process Count
key: PISTON_MAX_PROCESS_COUNT
default: 64
Maximum number of processes allowed to to have open for a job.

Resists against exhausting the process table, causing a full system lockup.

Output Max Size
key: PISTON_OUTPUT_MAX_SIZE
default: 1024
Maximum size of stdio buffers for each job.

Resist against run-away output which could lead to memory exhaustion.

Max Open Files
key: PISTON_MAX_OPEN_FILES
default: 64
Maximum number of open files at a given time by a job.

Resists against writing many smaller files to exhaust inodes.

Max File Size
key: PISTON_MAX_FILE_SIZE
default: 10000000 #10MB
Maximum size for a singular file written to disk.

Resists against large file writes to exhaust disk space.

Compile/Run timeouts
key:
  - PISTON_COMPILE_TIMEOUT
default: 10000

key:
  - PISTON_RUN_TIMEOUT
default: 3000
The maximum time that is allowed to be taken by a stage in milliseconds. Use -1 for unlimited time.

Compile/Run memory limits
key:
    - PISTON_COMPILE_MEMORY_LIMIT
    - PISTON_RUN_MEMORY_LIMIT
default: -1
Maximum memory allowed by a stage in bytes. Use -1 for unlimited memory usage.

Useful for running memory-limited contests.

Repository URL
key: PISTON_REPO_URL
default: https://github.com/engineer-man/piston/releases/download/pkgs/index
URL for repository index, where packages will be downloaded from.

Maximum Concurrent Jobs
key: PISTON_MAX_CONCURRENT_JOBS
default: 64
Maximum number of jobs to run concurrently.

Limit overrides
key: PISTON_LIMIT_OVERRIDES
default: {}
Per-language overrides/exceptions for the each of max_process_count, max_open_files, max_file_size, compile_memory_limit, run_memory_limit, compile_timeout, run_timeout, output_max_size. Defined as follows:

PISTON_LIMIT_OVERRIDES={"c++":{"max_process_count":128}}
This will give c++ a max_process_count of 128 regardless of the configuration.
API
Piston exposes an API for managing packages and executing user-defined code.

The API is broken in to 2 main sections - packages and jobs.

The API is exposed from the container, by default on port 2000, at /api/v2/.

All inputs are validated, and if an error occurs, a 4xx or 5xx status code is returned. In this case, a JSON payload is sent back containing the error message as message

Runtimes
GET /api/v2/runtimes
Returns a list of available languages, including the version, runtime and aliases.

Response
[].language: Name of the language
[].version: Version of the runtime
[].aliases: List of alternative names that can be used for the language
[].runtime (optional): Name of the runtime used to run the langage, only provided if alternative runtimes exist for the language
Example
GET /api/v2/runtimes
HTTP/1.1 200 OK
Content-Type: application/json

[
  {
    "language": "bash",
    "version": "5.1.0",
    "aliases": ["sh"]
  },
  {
    "language": "javascript",
    "version": "15.10.0",
    "aliases": ["node-javascript", "node-js", "javascript", "js"],
    "runtime": "node"
  }
]
Execute
POST /api/v2/execute
Runs the given code, using the given runtime and arguments, returning the result.

Request
language: Name or alias of a language listed in runtimes
version: SemVer version selector of a language listed in runtimes
files: An array of files which should be uploaded into the job context
files[].name (optional): Name of file to be written, if none a random name is picked
files[].content: Content of file to be written
files[].encoding (optional): The encoding scheme used for the file content. One of base64, hex or utf8. Defaults to utf8.
stdin (optional): Text to pass into stdin of the program. Defaults to blank string.
args (optional): Arguments to pass to the program. Defaults to none
run_timeout (optional): The maximum allowed time in milliseconds for the compile stage to finish before bailing out. Must be a number, less than or equal to the configured maximum timeout.
compile_timeout (optional): The maximum allowed time in milliseconds for the run stage to finish before bailing out. Must be a number, less than or equal to the configured maximum timeout. Defaults to maximum.
compile_memory_limit (optional): The maximum amount of memory the compile stage is allowed to use in bytes. Must be a number, less than or equal to the configured maximum. Defaults to maximum, or -1 (no limit) if none is configured.
run_memory_limit (optional): The maximum amount of memory the run stage is allowed to use in bytes. Must be a number, less than or equal to the configured maximum. Defaults to maximum, or -1 (no limit) if none is configured.
Response
language: Name (not alias) of the runtime used
version: Version of the used runtime
run: Results from the run stage
run.stdout: stdout from run stage process
run.stderr: stderr from run stage process
run.output: stdout and stderr combined in order of data from run stage process
run.code: Exit code from run process, or null if signal is not null
run.signal: Signal from run process, or null if code is not null
compile (optional): Results from the compile stage, only provided if the runtime has a compile stage
compile.stdout: stdout from compile stage process
compile.stderr: stderr from compile stage process
compile.output: stdout and stderr combined in order of data from compile stage process
compile.code: Exit code from compile process, or null if signal is not null
compile.signal: Signal from compile process, or null if code is not null
Example
POST /api/v2/execute
Content-Type: application/json

{
  "language": "js",
  "version": "15.10.0",
  "files": [
    {
      "name": "my_cool_code.js",
      "content": "console.log(process.argv)"
    }
  ],
  "stdin": "",
  "args": ["1", "2", "3"],
  "compile_timeout": 10000,
  "run_timeout": 3000,
  "compile_memory_limit": -1,
  "run_memory_limit": -1
}
HTTP/1.1 200 OK
Content-Type: application/json

{
  "run": {
    "stdout": "[\n  '/piston/packages/node/15.10.0/bin/node',\n  '/piston/jobs/e87afa0d-6c2a-40b8-a824-ffb9c5c6cb64/my_cool_code.js',\n  '1',\n  '2',\n  '3'\n]\n",
    "stderr": "",
    "code": 0,
    "signal": null,
    "output": "[\n  '/piston/packages/node/15.10.0/bin/node',\n  '/piston/jobs/e87afa0d-6c2a-40b8-a824-ffb9c5c6cb64/my_cool_code.js',\n  '1',\n  '2',\n  '3'\n]\n"
  },
  "language": "javascript",
  "version": "15.10.0"
}
Packages
GET /api/v2/packages
Returns a list of all possible packages, and whether their installation status.

Response
[].language: Name of the contained runtime
[].language_version: Version of the contained runtime
[].installed: Status on the package being installed
Example
GET /api/v2/packages
HTTP/1.1 200 OK
Content-Type: application/json

[
  {
    "language": "node",
    "language_version": "15.10.0",
    "installed": true
  },
  {
    "language": "bash",
    "language_version": "5.1.0",
    "installed": true
  }
]
POST /api/v2/packages
Install the given package.

Request
language: Name of package from package list
version: SemVer version selector for package from package list
Response
language: Name of package installed
version: Version of package installed
Example
POST /api/v2/packages
Content-Type: application/json

{
  "language": "bash",
  "version": "5.x"
}
HTTP/1.1 200 OK
Content-Type: application/json

{
  "language": "bash",
  "version": "5.1.0"
}
DELETE /api/v2/packages
Uninstall the given package.

Request
language: Name of package from package list
version: SemVer version selector for package from package list
Response
language: Name of package uninstalled
version: Version of package uninstalled
Example
DELETE /api/v2/packages
Content-Type: application/json

{
  "language": "bash",
  "version": "5.x"
}
HTTP/1.1 200 OK
Content-Type: application/json

{
  "language": "bash",
  "version": "5.1.0"
}
