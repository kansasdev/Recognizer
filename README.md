# Recognizer
Project for OCR and speech using cloud services from Azure.
Intention of project was to see if Xamarin can be usable for shared code projects.
From my perspective, if your application is quite typical (CRUD or something like 3 layer app) - Xamarin works.
Additionally, if your first language is C# - it is even better for you to use Xamarin.
I have been trying to create some applications with not my first choice language. It was so hard for me
to do even simple app in other ecosystem. So I gave xamarin a try - and for my purpose it was fine.
Application is working only if you have Azure subscription. You will need:
* Cognitive Services - vision (key,endpoint)
* Cognitive Services - speech (key,region)
* You can set up also different than en-US language for recognition (only Azure supported languages work)

Of course this application is heavily dependent on other libraries:
* Newtonsoft.Json
* Acr.UserDialogs
* Xam.Plugins.Settings
* Plugin.Permissions

