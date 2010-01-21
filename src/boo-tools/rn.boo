import System
import System.IO
import System.Text.RegularExpressions

if argv.Length != 2:
  print "rn: rename files in the current directory using regular expressions."
  print "Usage:   rn <pattern> <replacement>"
  print
  print "Example: rn \"^(.*)$\" \"old_$1\""
  return

pattern = argv[0]
replacement = argv[1]

for file in DirectoryInfo(".").GetFiles():
  continue if not Regex.IsMatch(file.Name, pattern)
  name = Regex.Replace(file.Name, pattern, replacement)
  destination = Path.Combine(file.DirectoryName, name)
  if Directory.Exists(".svn"):
    print shell("svn", "move ${file.Name} ${name}")
  else:
    File.Move(file.FullName, destination)
