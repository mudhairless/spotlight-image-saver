# Spotlight Image Saver
Saves the Windows 10 Spotlight Images to a folder for
use as backgrounds.

[![Build status](https://ci.appveyor.com/api/projects/status/h68ehqu2bra1g1ej?svg=true)](https://ci.appveyor.com/project/mudhairless/spotlight-image-saver)

## Usage (GUI)
1. Open the program
2. Set the save directory with the button
3. The close button minimizes the app to the tray, use File > Exit to really close.
4. Enjoy notifications when the app finds new pictures.

## Usage (Command Line)
1. Create the Directory to save images to.
2. Open a command prompt.
3. Run this command:

`spotimgsave.exe "C:\Path\ToSave\Images"`

4. You may now close the command prompt.

### Command Line Reference

#### Usage
`spotimgsave.exe path/to/save/files/to [--only-desktop|--only-mobile]`

#### Supported Commands
- --only-desktop - Only copy files in landscape orientation
- --only-mobile  - Only copy files in portrait orientation

Each option is exclusive cannot be combined with the other.

#### Environment Variables
- VERBOSE - if set to a non-empty value you will get more output
- DRYRUN - if set to a non-empty value no copying will take place just a list of what would've been copied.

#### Example
__Command Prompt:__
```
set DRYRUN=1

set VERBOSE=1

spotimgsave save/here
```

Clear variables with:
`set DRYRUN=`

__Bash/MSYS:__
```bash
DRYRUN=1 VERBOSE=1 spotimgsave.exe save/here
```

##### Covering my butt
_Windows and Windows 10 are Registered Trademarks of Microsoft and is used without permission._
