The project has been build using .NET core 2.1 and Visual Studio 2017 version 15.9.4. 
It has one service layer and one presentation layer which is a console. 
Besides, one Unit test project and one Integration test project are added. 
The presentation layer named as "OfferFinderConsole", service layer named as "OfferService" and Model library named as "Models" is mostly commented. 
Unit and Integration test projects are not commented at all.

To run the project from Command Line
Open the command line and locate to the OfferFinderConsole.dll folder. Then run any of the following commands
dotnet OfferFinderConsole.dll amsterdam,tuin 10
dotnet OfferFinderConsole.dll amsterdam,tuin  => As default is 10 to be shown
dotnet OfferFinderConsole.dll amsterdam 15