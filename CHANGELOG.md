# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0.0-pre.4] - 2021-08-26

### Fixed

* notificationOpened analytics events will work correctly on Android
* The host Android app will not forceably reboot if opened from background from a notification
* Updated analytics integration so that the userCountry is correctly reported for push events
* Documentation improvements

## [1.0.0-pre.3] - 2021-08-19

### Fixed

* Updated 3rd party code notices

## [1.0.0-pre.2] - 2021-08-17

### New Features

* The package now uses the Core initialisation flow.

### Fixed

* Subsequent updates of the Android token will no longer cause a null pointer exception

## [1.0.0-pre.1] - 2021-08-05

### New Features

* Support for Android devices, including rich push notifications
* A simplified, unified external interface that allows one registration for both platforms

### Fixed

* Notifications now show when app is in the foreground on iOS

## [0.1.0-preview] - 2021-07-27

### New Features

* Support for iOS platform, including rich push notifications
* Push notification analytics - notificationOpened and notificationServices events

### Known Issues

* Android is not currently supported
* Analytics platform is hard coded to "IOS" as this integration is changing in the next release and currently this is the only platform we support
* Analytics country is hard coded to "GB" as this integration is changing in the next release and the currently released Analytics package doesn't provide this API
