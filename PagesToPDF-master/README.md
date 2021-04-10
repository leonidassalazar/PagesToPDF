# PagesToPDF
This is a simple project that download the novels' chapters from a website and save it in pdf.

It's possible save chapters, volumes and the novel, separately, to do this, got to appsettings.json and set true to the following properties:
'saveChapters', 'saveBook' or/and 'saveVolumes'.

The propety 'saveLocal', is the place where the files will be saved.

The property 'novels' is a array, each its elements must have 'Title' ana 'Code' values, 'Title' is the novel title and 'Code' is de novel's code, can be found after the last '/' from novel's url.
