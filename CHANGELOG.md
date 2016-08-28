Changelog for HandbookApp
=========================


2016-05-05: Initial commit
--------------------------

- Initial commit
- Apache 2.0 License
- Readme
- ChangeLog


2016-08-05: Release 1.0a
------------------------

- First alpha release to residents



2016-08-XX: Release 1.0a2
-------------------------

1. Random iOS Unauthorized access
	- Not fixed yet

2. Unable to Logout, Reset Licence Key or Reset Contents
	- Created Settings Page with access from MainPage
	- Created Logout, Reset Licence Key and Reset Contents functionality on SettingsPage
	* Reseting Licence Key needs to go back to MainPage (still in rough)
	
3. On resource limited iOS, Offline save state does not finish or OnSleep does not get called.
	- Possibly because of iOS rules for background tasks that the app is simple terminated
	- Inserted an offline save state at the end of the Full Updates in the ServerService
	
 