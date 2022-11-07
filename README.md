# PoliDL-GUI

Download: https://github.com/yuyu-19/PoliDL-GUI/releases

Note: A couple of antiviruses on VirusTotal incorrectly flag this as malware. As far as I can tell, this is due to the program extracting the required DLLs in the folder it's running in. You can check the source code yourself and build it to make sure.

### English version below!

Un grande grazie a @sup3rgiu per PoliWebex e PoliDown (su cui si basa l'intera funzionalità di download) e per avermi aiutato con le modifiche necessarie.

PoliWebex: https://github.com/sup3rgiu/PoliWebex
PoliDown: https://github.com/sup3rgiu/PoliDown

## Italiano

Richiede .NET framework v4.6

Questo programma permette di scaricare molteplici lezioni da Webex
 
### Download
Se esiste già il file corrispondente ad un video il download continuerà da dove è stato interrotto, saltandolo nel caso il file sia stato scaricato completamente.

Una volta che il programma ha cercato di scaricare tutti i link forniti verrà chiesto all'utente se vuole riprovare a scaricare quelli falliti.

Il programma può estrarre link di webex da un paio di tipi di file:
  - .xlsx
  - .docx
  - .html
  - .zip

Puoi selezionare un singolo file o una cartella, o inserire i link manualmente.

Se selezioni una cartella, assicurati di includere solo i tipi di file supportati in essa e nelle sue sottocartelle.



I zip file possono essere "nested", cioè contenere altri zip file al loro interno.

Questo significa che, ad esempio, se un professore avesse una struttura di cartelle (su beep) del genere:

 - Lezioni Online
 	- Lezione 1
 		- .docx/.xlsx file contenente un link
	- Lezione 2
		- .docx/.xlsx file contenente un link
	- Lezione 3
		- .docx/.xlsx file contenente un link
      

In questo esempio scaricare (cliccando "scarica cartella") e dare al programma il file zip di "Lezioni online" funzionerebbe.


## English

Requires .NET framework v4.6

A huge thank you to @sup3rgiu for PoliWebex and PoliDown (on which the entire download functionality is based on) and for helping me make the necessary changes.

PoliWebex: https://github.com/sup3rgiu/PoliWebex
PoliDown: https://github.com/sup3rgiu/PoliDown

This tool allows you to donwload lessons from Webex in bulk

### Download
If the video file already exists, the download will be resumed from where it was interrupted, and skipped entirely if it was already completed.

Once the program has attempted to download all the given videos, it will prompt the user and ask them if they would like to retry the failed ones.

The program can extract webex links from a couple of filetypes:
  - .xlsx
  - .docx
  - .html
  - .zip
  
 You can either select a single file or a folder as input, or simply input the links manually. 
 If you select a folder, take care to only include supported filetypes in it and its subfolders.
 
 The zip files can be nested, as in contain other zip files within them.
 
 This means that, for example, if a professor were to have a file structure (on beep) like this:
- Online lessons
	- Lesson 1
		- .docx/.xlsx file containing a link
	- Lesson 2
		- .docx/.xlsx file containing a link
	- Lesson 3
		- .docx/.xlsx file containing a link

Downloading (by clicking "download folder") and giving the program the zip file of "Online lessons" would work.
