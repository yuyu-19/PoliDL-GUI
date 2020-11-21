# PoliWebex-GUI
English version below!

## Italian

Questo programma ha due funzionalità principali:
  -Registrare lezioni in locale automaticamente (Locale)
  -Scaricare molteplici 
### Download






## English

This tool has two main functions:
  -Downloading lessons from Webex in bulk   (Download)
  -Automatically recording lessons locally  (Local)

### Download
It can extract webex links from a couple of filetypes:
  -.xlsx
  -.docx
  -.html
  -.zip
  
 You can either select a single file or a folder as input, or simply input the links manually. 
 Take care to only include supported filetypes in the folder and its subfolders.
 
 The zip files can be nested, as in contain other zip files within them.
 This means that, for example, if a professor were to have a file structure like this:
 -Online lessons
    -Lesson 1
      -.docx/.xlsx file containing a link
    -Lesson 2
      -.docx/.xlsx file containing a link
    -Lesson 3
      -.docx/.xlsx file containing a link

Downloading and giving the program the zip file of "Online lessons" by clicking download folder would work.

### Local
Its functionality is a bit complex:
  -It takes the lesson calendar (online services > timetable > right click and download as HTML only) as input.
  -It will then prompt you to select which professors will be giving which lesson (can be edited later)
  -Once that's done, it will set up tasks using Windows' task scheduler.
  -These tasks will run when a lesson is supposed to start, join the room and begin recording to the folder you selected earlier.
 
 You can edit the teacher that is giving a lesson and delete them by entering local mode after the first time setup.
 You are also able to:
  -Disable the recording for the next time (In case a lesson is rescheduled).
  -Add a "one time" recording, which will only run once.
 
 Tasks will be deleted once the semester is over.
