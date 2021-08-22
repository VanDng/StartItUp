# StartItUp

### 1. Introduction
An application that grogrammatically executes other scripts/apps.
It has a list of available profiles which are developed to interact with certain apps/scripts.

<p align="center">
    <img src="/Doc/Images/main.png" alt="Main App">
</p>

This app comes from my very personal reason. While I was working in different projects, I usually had to run some scripts/apps to make my development environment be ready whenever I turned on my computer.
Some apps does not noly require to be executed but perform some clicks under some certion conditions.

I had used to write different apps/scripts to automatically do all these things, but all implementations had been seperated, I had not reused anything.
I decided to create an app with a long-term enhencement, I do not have to write everything from scratch again, so I chose to make it extendable by linking with dynamic libraries.

That is all about why this app has been developed and maintained.

Currently, there are two profiles:
  - OpenVPN
      - It executes OpenVPN-GUI app, and run some commands to start connecting to a profile that was imported and connected successfully already.
      - It does not require/store the user's credential, credential must be entered to OpenVPN-GUI and Remember Password is checked.
  - Worksnaps:
      - It execute Worksnaps app, and perform some clicks to start recording.
      - It does not require/store the user's credential, credential must be entered to OpenVPN-GUI and Remember Password is checked.

<p align="center">
    <img src="/Doc/Images/demo.gif" alt="Demo OpenVPN">
</p>

### 2. Credit
The app's icon is downloaded from https://www.pinclipart.com/pindetail/xRiTRJ_launch-launch-icon-clipart/
