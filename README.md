# ezclap - PoC

EZClap is a Windows Userland persistence which was created for educational purposes in Red vs. Blue team competitions. As the tool is targetted towards beginners in Windows Security, most of the mechanisms are easy to detect through usage of sysinternal tools or simple cmd/powershell commands.

## Disclaimer
EZClap is a proof of concept tool which was only created for educational purposes in classroom and cybersecurity related competitions. This tool is not created, nor is good enough for any real world usage. I do not condone use of this tool anything other than educational purposes. Using any of the files of yabnet in/against a system that you do not own is illegal, and you will get caught.

## Under Construction - PoC 
**As I have only started to learn C# about 1 week ago, EZClap is in a horrible PoC state.** 

- [ ] Refactor code 
- [ ] Support for fileless payload 
- [ ] Implement more modules - Startup file, bitsadmin, etc 
- [ ] Test with various payloads, not just yabnet payloads 
- [ ] Get some feedback from colleagues because chances are this is a horrible tool 

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
`./ezclap.exe 'C:\Users\Administrator\Desktop\payload.exe` 

## Demo 

**If you have an .exe payload, that's great. If not, or if you are feeling lazy, let's just use cmd.exe.**

1. Download `ezclap.exe` from the releases page and transfer to the target

2. Use `cmd.exe` or `powershell.exe` 

`./ezclap 'C:\windows\system32\cmd.exe'`

3. Reboot 

4. Through ProcessExplorer or Task Manager, see that cmd.exes are being ran 

5. See backdoor users being added 

`net users` 



