Instructions: 
--------------------------------------------------------
Run Backend: 
- Open Solution in Visual Studio
  - All API Code is in the "webapi" Project
- Run App with F5

Run Frontend: 
- open "frontend" folder in VS Code
- execute "npm install" in Terminal 
- execute "npm run serve" in Terminal to open Page

-> Important: Use Test Page to Add new Activity Entries (or new Player using LogType = None) 
--------------------------------------------------------

Overview: 
--------------------------------------------------------
-> HomePage is the Leaderboard which is a cached version received from the API which calculates it by looking at "Earned Points" Activities
(the cache is refreshed after 1 minute for testing Purposes and will be invalidated once a new activity or player has been added).

-> To see Player Details, click on a Player Name.

-> On the Detail Page the Score calculation can be filtered by using the DateTime Fields, only Activities within the given are calculated, empty fields count as DateTime.MinValue or DateTime.MaxValue respectively.

-> On the Activities Page all PlayerActivities are listed, by Typing a PlayerUUID into the InputField, a filter is applied.

-> On the Test Page it's possible to add new Activities and/or Players. Depending on what is selected in the DropDown, the input Fields change.
--------------------------------------------------------
