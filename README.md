# StartItUp

### 1. Introduction
An application that programmatically executes other scripts/apps.
It has a list of available profiles which are developed to interact with certain apps/scripts.

<p align="center">
    <img src="/Doc/Images/main.png" alt="Main App">
</p>

This app comes from my very personal reason. While I was working on different projects, I usually had to run some scripts/apps to make my development environment be ready whenever I turned on my computer.
Some apps do not only require to be executed but perform some clicks under certain conditions.

I had had to write different apps/scripts to automatically do all these things, but all implementations had been separated, I had not reused anything.
Until recently, I decided to create an app with a long-term enhancement, I do not have to write everything from scratch again, so I chose to make it extendable by developing the app with dynamic libraries.

That is all about why this project has been started.

Currently, there are two profiles:
  - OpenVPN
      - It executes the OpenVPN-GUI app and runs some commands to start connecting to a profile that was imported and connected successfully already.
      - It does not require/store the user's credential, the credential must be entered to OpenVPN-GUI, and Remember Password is checked.
  - Worksnaps
      - It executes the Worksnaps app and performs some clicks to start recording.
      - It does not require/store the user's credential, the credential must be entered to OpenVPN-GUI, and Remember Password is checked.

<p align="center">
    <img src="/Doc/Images/demo.gif" alt="Demo OpenVPN">
</p>

### 2. Credit
The app's icon is downloaded from https://www.pinclipart.com/pindetail/xRiTRJ_launch-launch-icon-clipart/
