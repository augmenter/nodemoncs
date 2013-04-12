nodemoncs
=========

C# simplified version of nodemon

Fully based on https://gist.github.com/bennage/1108727,
with added command line parameters, recursive directory monitoring, and 
monitoring of all files, not just *.js.

Usage example:

nodemon --path=C:\node\project1 --startup=C:\node\project1\app.js
      
--path : path to monitor
--startup : main project node script

Update 4.12.2013: Added node error output to console, and R + [ENTER] option to restart node by hand.