For run this application without errors:

#1 - check if your connectionstrings are in use! (if you do not have them, make them, connectionstring to the database showing up in SSMS)
# Connectionstring names should be the same as it written in the program.cs Connectionstrings can be found now in appsettings.json


#2 - Make add-migration -context {DbContext} to the new db contexts if you did not had them earlier. Database contexts are under Data folder (new database context on 04.04.2024 -> Job Offer DbContext)
# (remember : packet manager console!)

#3 - Make update-database -context {DbContext}  (the db contexts you need to update)


#4 Run the application