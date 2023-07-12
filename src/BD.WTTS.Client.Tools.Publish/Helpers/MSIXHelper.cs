namespace BD.WTTS.Client.Tools.Publish.Helpers;

static class MSIXHelper
{
    static void W(Stream stream, string version)
    {
        // https://learn.microsoft.com/zh-cn/windows/msix/desktop/desktop-to-uwp-manual-conversion
        var appxmanifest =
"""
﻿<?xml version="1.0" encoding="utf-8"?>
<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  xmlns:desktop="http://schemas.microsoft.com/appx/manifest/desktop/windows10"
  xmlns:desktop2="http://schemas.microsoft.com/appx/manifest/desktop/windows10/2"
  IgnorableNamespaces="uap rescap desktop desktop2">
  <Identity
    Name="4651ED44255E.47979655102CE"
    Publisher="CN=A90E406B-B2D3-4A23-B061-0FA1D65C4F66"
    Version="{0}"
    ProcessorArchitecture="x64" />
  <Properties>
    <DisplayName>Watt Toolkit</DisplayName>
    <PublisherDisplayName>软妹币玩家</PublisherDisplayName>
    <Logo>Images\StoreLogo.png</Logo>
  </Properties>
  <Dependencies>
    <TargetDeviceFamily Name="Windows.Universal" MinVersion="10.0.17763.0" MaxVersionTested="10.0.19041.0" />
    <TargetDeviceFamily Name="Windows.Desktop" MinVersion="10.0.17763.0" MaxVersionTested="10.0.19041.0" />
  </Dependencies>
  <Resources>
    <Resource Language="x-generate"/>
  </Resources>
  <Applications>
    <Application Id="App"
      Executable="$targetnametoken$.exe"
      EntryPoint="$targetentrypoint$">
      <uap:VisualElements
        DisplayName="Watt Toolkit"
        Description="「Watt Toolkit」是一个开源跨平台的多功能游戏工具箱，此工具的大部分功能都是需要您下载安装 Steam 才能使用。"
        BackgroundColor="transparent"
        Square150x150Logo="Images\Square150x150Logo.png"
        Square44x44Logo="Images\Square44x44Logo.png">
        <uap:DefaultTile Wide310x150Logo="Images\Wide310x150Logo.png"  Square71x71Logo="Images\SmallTile.png" Square310x310Logo="Images\LargeTile.png" ShortName="Steam++">
          <uap:ShowNameOnTiles>
            <uap:ShowOn Tile="square150x150Logo"/>
            <uap:ShowOn Tile="wide310x150Logo"/>
            <uap:ShowOn Tile="square310x310Logo"/>
          </uap:ShowNameOnTiles>
        </uap:DefaultTile >
        <uap:SplashScreen Image="Images\SplashScreen.png" />
        <uap:InitialRotationPreference>
          <uap:Rotation Preference="landscape"/>
        </uap:InitialRotationPreference>
        <uap:LockScreen BadgeLogo="Images\BadgeLogo.png" Notification="badgeAndTileText"/>
      </uap:VisualElements>
      <Extensions>
        <desktop:Extension Category="windows.fullTrustProcess" Executable="Steam++\Steam++.exe" />
        <desktop:Extension Category="windows.startupTask" Executable="Steam++\Steam++.exe" EntryPoint="Windows.FullTrustApplication">
          <desktop:StartupTask TaskId="BootAutoStartTask" Enabled="true" DisplayName="Steam++ System Boot Run" />
        </desktop:Extension>
      </Extensions>
    </Application>
  </Applications>
  <Extensions>
    <desktop2:Extension Category="windows.firewallRules">
      <desktop2:FirewallRules Executable="Steam++\Steam++.exe">
        <desktop2:Rule Direction="in" IPProtocol="TCP" Profile="all" />
        <desktop2:Rule Direction="in" IPProtocol="UDP" Profile="all" />
      </desktop2:FirewallRules>
    </desktop2:Extension>
  </Extensions>
  <Capabilities>
    <Capability Name="internetClient" />
    <rescap:Capability Name="runFullTrust" />
    <rescap:Capability Name="allowElevation" />
  </Capabilities>
</Package>
"""u8;
    }

    enum WinVersion : ushort
    {
        W10_20H1 = 19041,
        W11_21H2 = 22000,
        W11_22H2 = 22621,
    }

    static class MakeAppx
    {
        // https://learn.microsoft.com/zh-cn/windows/msix/package/create-app-package-with-makeappx-tool
        // 使用 MakeAppx.exe 创建 MSIX 包或捆绑包

        const string makeappx_exe = "makeappx.exe";

        static string GetMakeAppxPath(WinVersion version)
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            path = Path.Combine(path, "Windows Kits", "10", "bin", $"10.0.{(ushort)version}.0", RuntimeInformation.OSArchitecture.ToString().ToLowerInvariant(), makeappx_exe);
            return path;
        }

        static string GetMakeAppxPath()
        {
            var query = from m in Enum.GetValues<WinVersion>().OrderByDescending(x => (ushort)x)
                        let p = GetMakeAppxPath(m)
                        let exists = File.Exists(p)
                        where exists
                        select p;
            var path = query.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(path))
                throw new FileNotFoundException(makeappx_exe);
            return path;
        }

        public static void Start(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = GetMakeAppxPath(),
                UseShellExecute = false,
                Arguments = arguments,
            };
            DotNetCLIHelper.StartProcessAndWaitForExit(psi);
        }
    }
}
