# GitJanitor

GitJanitor is a Work in Progress .NET 7 CLI tool to manage Git repositories. 
It allows developers to streamline their workflow by simplifying the management of repositories.

## Installation

You can install GitJanitor using the .NET 7 CLI:

```bash
dotnet tool install --global GitJanitor
```
Usage
```bash
gitjanitor --action=<action> --path=<path> --owner=<owner> --target=<target>
```

### Where:

- --action: Specify the action to be performed. Available actions include Archive, Delete and Move. This flag is required.
- --path: Specify the local directory path where actions are to be performed. This flag is required.
- --owner: Specify the GitHub owner name. this is used as a filter for repositories This flag is (for now) required.
- --target: Specify the destination directory. This flag is required if the Action is either Move or Archive.

## Examples
#### To archive a repository:
```bash
gitjanitor --action=Archive --target=my-repo
```
#### To delete a repository:
```bash
gitjanitor --action=Delete --target=my-repo
```
#### To move a repository from a local directory to another:

```bash
gitjanitor --action=Move --path=C:\projects\test-project --target=C:\Users\username\Desktop
```

### License
GitJanitor is licensed under the MIT license.

### Important
GitJanitor is a Work In Progress project. Use with caution in production environments. Remember to always back up your data.
<br>
Please note that as GitJanitor is still under development, everything is still subject to change - including the available flags, configurations, and more.

### Disclaimer
The creators and maintainers of GitJanitor are not responsible for any data loss or other damages that may result from using the tool. Always use good judgement and discretion when dealing with important data and repositories.