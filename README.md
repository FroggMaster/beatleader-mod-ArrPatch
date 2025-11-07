# BeatLeader-ArrPatch PC mod

An open-source leaderboard System for Beat Saber with the trust issues removed.
- ~~[Join our Discord server](https://discord.gg/2RG5YVqtG6)~~ (The BeatLeader developer and admins are uptight, if you mention cracking or pirating in anyway they will ban you from the Discord and Beatleader. Don't say I didn't warn you.)

## Usage

- Download the zip for your game version from the [Releases](https://github.com/FroggMaster/beatleader-mod-ArrPatch/releases/) and extract it to your BeatSaber directory
- Rename the `Example_BeatLeader_LoginCookie.json` to `BeatLeader_LoginCookie.json` in **\<BeatSaberDir\>\UserData\BeatLeader**
- Navigate to [BeatLeader Signin API Page](https://api.beatleader.com/signin)
- Open the Developer console of your web browser with **Ctrl+Shift+I**
- In the developer console click the Network Tab
- Click Connect using <Preferred Platform>
- Go through the sign-in process as normal
- Once you've signed in you will need to copy the Cookie Value for your login. You'll be looking for the request that starts with ```signin-```
I've attached an example of what that looks like for the Steam login. <img width="2733" height="535" alt="image" src="https://github.com/user-attachments/assets/10178b60-3ce9-47f1-83c5-695217334385" />
- Inside of `BeatLeader_LoginCookie.json` you'll need to add your cookieValue
    ```
    {
      "cookieValue": "Put_Your_Cookie_Value_Here"
    }
    ```
- Make sure to install and update all required [dependencies](#dependencies)

Go to the https://beatleader.xyz/ to see your scores on the web

DO NOT DISCUSS ARRPATCH IN THE BEATLEADER DISCORD. If you have issues with logging on VIA `BeatLeader_LoginCookie.json` please create a [Github Issue](https://github.com/FroggMaster/beatleader-mod-ArrPatch/issues).
For any issues actually related to BeatLeader go talk to their uptight admins.
~~If you experience any issues, have any suggestions or bug reports - you can leave them in our [Discord server](https://discord.gg/2RG5YVqtG6)~~

# Build Instructions
<details>
<summary><strong>Method A</strong></summary>

- Create a `BeatLeader.csproj.user` file that contains the following:
    ```xml
    <Project>
      <PropertyGroup>
        <!-- Change this path to your actual Beat Saber folder -->
        <BeatSaberDir>PUT_THE_PATH_TO_YOUR_BEATSABER_INSTALL_HERE</BeatSaberDir>
      </PropertyGroup>
    </Project>
    ```
- Make sure you've downloaded the required [dependencies](#dependencies) and installed them to Beat Saber

</details>

<details>
<summary><strong>Method B</strong></summary>

- Make sure you've downloaded the required [dependencies](#dependencies) and installed them to Beat Saber
- Create a `Refs\` folder in the root of the repository
- Copy your entire Beat Saber install into the `Refs\` folder
</details>


## Dependencies

- BSIPA, BSML, SiraUtil - available in [BSManager](https://github.com/Zagrios/bs-manager) and on the [BeatMods website](https://beatmods.com/#/mods)
- ReactivePlatform - available in [Reactive Platform beat-saber-sdk](https://github.com/reactive-platform/beat-saber-sdk/releases/)
- [LeaderboardCore](https://github.com/rithik-b/LeaderboardCore)

