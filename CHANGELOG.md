# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [3.0.0-pre.1] - 2022-12-16

### Fixed
Fixed a bug preventing the `notificationOpened` event from being sent after opening a notification while the app was fully closed

### Changes

- The following classes have been changed to `internal`:
  - `InsertPushNotificationDependenciesIntoGradleScript`
  - `IOSRichPushNotificationPostProcess`
  - `IOSBasicPushNotificationPostProcess`
  - `PushSettingsProvider`
  - `PushNotificationEditorGameService`
  - `PushNotificationAnalytics`
- Moved `IPushNotificationsAnalytics` to it's own file.
- Change `PushNotificationsService` to use the `IPushNotificationsAnalytics` interface instead of 
directly using the `PushNotificationsAnalytics` class.
- Bumped the Analytics SDK version from `4.0.0-pre.2` to `4.3.0`
- Bumped the Core Package SDK version from `1.2.0` to `1.4.0`
- Dashboard project link now available in settings panel
- Renamed the following fields in `PushNotificationSettings` (Values will be retained):
  - `androidApiKey` -> `firebaseWebApiKey`
  - `androidSenderId` -> `firebaseProjectNumber`
  - `androidApplicationId` -> `firebaseAppID`
  - `androidProjectId` -> `firebaseProjectID`

### Removed
- Removed the deprecated `PushNotifications` class.
- Removed obsolete `PushNotificationSettings` properties.

### Known Issues
- Notifications received while the app is opened will automatically trigger the `notificationOpened` event even before they are opened
- `didLaunch` property sent with `notificationOpened` event not fully accurate
- Devices running Android 13 will not receive notifications until the player grants permission. Permission won’t be requested until the device receives a notification for the first time.

## [2.0.0-pre.2] - 2022-05-09

### Fixed

* Fixed a documentation bug
* Fixed a compile issue that could occur if custom Editor classes were added to a Unity project

## [2.0.0-pre.1] - 2022-05-06

### New Features 

* The default API of the SDK has changed to align with our other UGS packages, and is now available via `PushNotificationsService.Instance` rather than `PushNotifications` as before. The previous API has been kept for backwards compatability, but will be removed in a future release.

### Fixed

* Notifications will be shown as expected when an Android app is completely closed

### Removed

* The deprecated in-code settings API has been removed - the SDK should be configured via the Project Settings UI added in 1.1.0-pre.1

## [1.1.0-pre.2] - 2022-03-16

### Fixed

* Android plugin now uses a named Firebase instance to avoid clashing with any default instances already in the project.

## [1.1.0-pre.1] - 2022-01-21

### New Features

* The package is now configurable in the Project Settings UI. This is the recommended way of configuring the SDK moving forwards. The previous method of providing a configuration to the `RegisterForPushNotificationsAsync` method is now deprecated, although will continue to function for the moment. (Note that if both code and UI settings are provided, the code settings will override the UI settings).
* The dependency on Unity Analytics has been updated to support version 3.0.0 with the new privacy flow. You will need to implement this as described in the Analytics documentation before push notification events will be sent.

### Fixed

* Android plugin now supports Android API versions 16+

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
