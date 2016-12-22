# README Selenium Bank Data Downloader

For running this project just fire up Visual Studio, clone the Git repo and start developing. The dependencies are grabbed via NuGet.

### Included Banks ###

* [DKB](https://www.dkb.de/banking)
* [Number26](https://my.number26.de)
* [Raiffeisen Österreich](https://banking.raiffeisen.at)
* [Renault Bank direkt](https://ebanking.renault-bank-direkt.at)
* [Santander Consumer Bank](https://service.santanderconsumer.at/eva)

## Projects

* DataDownloader.Common
    * provides settings, models and some common helpers
* DataDownloader.Handler
    * provides all the Selenium dependent classes, like the BankDownloadHandlerBase class and some custome Selenium stuff
* DataDownloader.Test
    * provides the unit testing infrastructure, especially the mocking for the settings via json file 
* DataDownloader.Ui
    * provides the user interface for the application
* KeePass
    * provides a wrapper for accessing a KeePass file and some extension methods for the KeePassLib

## Selenium

From time to time you have to update the Chrome driver as older versions are not supported any more by Google Chrome.
Do this by updating the NuGet dependency version. 

## Testing

1. Use the testsettings.changeme.json in the DataDownloader.Test project.
1. Rename it to testsettings.json and fill the settings with the according values.
1. Please make sure to not commit this file afterwards as it contains a KeePass password.
    * Best practice is to use a KeePass file not containing anything apart from the entries necessary.
    * Use a generated master password for this KeePass database.