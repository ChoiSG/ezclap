# ezclap - PoC

EZClap is a Windows Userland persistence which was created for educational purposes in Red vs. Blue team competitions. This tool is targeted towards beginners in Windows Security; most of the mechanisms are easy to detect through usage of sysinternal tools or simple cmd/powershell commands. The tool's purpose is to provide beginners easy implants to detect, in order to give them confidence and make them learn about basic Windows Userland Security.

## Dear Blueteamers

Yeah, you, who is stalking red teamer's repos (you should, btw). I make all of my tools public because I believe that blue teamers learning is more important than my implants getting caught during competitions. As long as you are learning how to detect my tools, prevent them, and delete them, you are perfectly fine. Please focus on learning, than winning competitions! (ofc win them as well) 

## Disclaimer
EZClap is a proof of concept tool which was only created for educational purposes in classroom and cybersecurity related competitions. This tool is not created, nor is good enough for any real world usage. I do not condone use of this tool anything other than educational purposes. Using any of the files of my tools in/against a system that you do not own is illegal, and you will get caught.

## Warning
As EZClap is built under student attack/defense in-mind, it is not operationally secure. Moreover, the tool modifies the target machine heavily, to the point where it can damage normal business operation. If you are using this against a client's machine, **DON'T.** 

## Under Construction - PoC 
**As I have only started to learn C# about 1 week ago, EZClap is in a horrible PoC state.** 

- [x] Refactor code 
- [ ] Support for fileless payload 
- [x] Implement more modules - Startup file, bitsadmin, etc 
- [x] Test with various payloads, not just yabnet payloads 
- [x] Get some feedback from colleagues because chances are this is a horrible tool 
- [ ] Add a way to remove all the persistence, to make it more operationally secure

## Persistence Mechanisms
The following lists are the userland persistence mechanisms that are currently implemented. 

* WMI
* Backdoor Users (AD and local)
* RunKey Registry 
* Scheduled Task
* Accessibility
* Userinit
* FailureCommand
* Image File Execution 
* Secret RunKey - NtSetValueKey native API
* Bitsadmin - **TODO**
* Startup Folder - **TODO** 

## Components 
**Program.cs** - Main class which calls all userland persistence modules and executes them.

**app.config** - Configuration file to change payload name, registry key name, value, backdoor user names, passwords, etc. 

**Add[module]** - Various userland persistence modules to be called through the `Program.cs` main class 

**RegistryKeys.cs + Utils.cs** - Used for hardcoding and utility. Ignore these. 

## Installation 

**Please use a testing VM with a snapshot for testing out EZClap !** 

#### 1. Building 
Simply download the .exe file from the releases page, or git clone the repo and build it locally.

#### 2. Configuration  
Configuration can be done through modifying the `app.config` file. Feel free to change the names, or add sections. If you are feeling lazy, feel free to use the default configuration file.

#### 3. Transfer EZClap with a `.exe` payload 
Sadly, EZClap currently only supports .exe payload, which means you do need to write the payload on-disk. Support for fileless payloads are coming. 

#### 4. Execute EZClap 
`./ezclap.exe -t all -b 'C:\Users\Administrator\Desktop\payload.exe` 

#### 5. Reboot the target and enjoy
![Enjoy](https://i.imgur.com/yzLlzYR.png)

## Usage 
EZClap uses a simply command line parser. Use `-h` to see the help message. There are only two options, and they are straight forward.

There are currently 9 persist methods: `wmi,user,schetask,access,userinit,failure,runkey,imagefile,secretkey`

**Using Binary Payload**

`./ezclap.exe -t all -b <binary_full_path>`

**Using .dll/MSBuild other ways**

`./ezclap.exe -t all -c <command_and_payload>` 

**Using specific techniques** 

`./ezclap.exe -t wmi,user,service -b <binary_full_path` 

`./ezclap.exe -t access,userinit,runkey -c <command_and_payload>`

## Demo 

**If you have an .exe payload, that's great. If not, or if you are feeling lazy, let's just use cmd.exe.**

1. Download `ezclap.exe` from the releases page and transfer to the target

2. Use `cmd.exe` or `powershell.exe` 

`./ezclap.exe -t all -b C:\windows\system32\cmd.exe`

3. Reboot (or not, if you are feeling lazy) 

4. Through ProcessExplorer or Task Manager, see that cmd.exes are being ran 

5. See backdoor users being added 

`net users` 



