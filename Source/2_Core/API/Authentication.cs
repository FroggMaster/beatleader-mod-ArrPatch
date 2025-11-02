using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BeatLeader.Utils;
using BeatLeader.WebRequests;
using BS_Utils.Gameplay;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;
using System.Net;

namespace BeatLeader.API {
    internal static class Authentication {
        #region Platform

        public enum AuthPlatform {
            Undefined,
            Steam,
            OculusPC
        }

        public static AuthPlatform Platform { get; private set; }

        public static void SetPlatform(AuthPlatform platform) {
            Platform = platform;
        }

        public static async Task<string?> PlatformTicket() {
            await GetUserInfo.GetUserAsync();

            var platformUserModel = Resources
                .FindObjectsOfTypeAll<PlatformLeaderboardsModel>()
                .Select(l => l._platformUserModel)
                .Last(x => x != null);

            var userInfo = await platformUserModel.GetUserInfo(CancellationToken.None);
            var tokenProvider = new PlatformAuthenticationTokenProvider(platformUserModel, userInfo);

            return Platform switch {
                AuthPlatform.Steam => (await tokenProvider.GetAuthenticationToken()).sessionToken,
                AuthPlatform.OculusPC => (await tokenProvider.GetXPlatformAccessToken(CancellationToken.None)).token,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #endregion

        #region Login

        private static TaskCompletionSource<bool> _taskSource = new();
        private static bool _signedIn;

        public static void ResetLogin() {
            WebRequestFactory.CookieContainer.SetCookies(new Uri(BLConstants.BEATLEADER_API_URL), "");
            _signedIn = false;
            _taskSource = new();
        }

        public static Task<bool> WaitLogin() {
            return _taskSource.Task;
        }

        public static async Task Login() {
            if (_signedIn) return;

            // 1) Try cookie-based login first
            if (TryCookieLogin()) {
                Plugin.Log.Info("Login via cookie file successful!");
                _signedIn = true;
                _taskSource.SetResult(true);
                return;
            }

            // 2) Fallback to platform login
            if (!TryGetPlatformProvider(Platform, out var provider)) {
                Plugin.Log.Debug("Login failed! Unknown platform");
                return;
            }

            var authToken = await PlatformTicket();
            if (authToken == null) {
                Plugin.Log.Debug("Login failed! No auth token");
                return;
            }

            var result = await AuthRequest.Send(authToken, provider!).Join();

            switch ((int)result.RequestStatusCode) {
                case 200:
                    Plugin.Log.Info("Login successful!");
                    _signedIn = true;
                    _taskSource.SetResult(true);
                    break;

                case BLConstants.MaintenanceStatus:
                    Plugin.Log.Debug("Login failed! Maintenance");
                    break;

                default:
                    Plugin.Log.Debug($"Login failed! status: {result.RequestStatusCode} error: {result.FailReason}");
                    break;
            }
        }

        private static bool TryGetPlatformProvider(AuthPlatform platform, out string? provider) {
            switch (platform) {
                case AuthPlatform.Steam:
                    provider = "steamTicket";
                    return true;

                case AuthPlatform.OculusPC:
                    provider = "oculusTicket";
                    return true;

                case AuthPlatform.Undefined:
                default:
                    provider = null;
                    return false;
            }
        }

        #endregion

        #region CookieLogin

        private const string CookieFileName = "BeatLeader_LoginCookie.json";

        private class LoginCookie {
            // Minimal JSON:
            // { "cookieValue": "<paste_cookie_value_here>" }
            public string cookieValue { get; set; } = "";
        }

        private static bool TryCookieLogin() {
            try {
                var path = Path.Combine(Environment.CurrentDirectory, "UserData", "BeatLeader", CookieFileName);
                if (!File.Exists(path)) return false;

                var json = File.ReadAllText(path);
                var cookieInfo = JsonConvert.DeserializeObject<LoginCookie>(json);
                if (cookieInfo == null || string.IsNullOrEmpty(cookieInfo.cookieValue)) {
                    Plugin.Log.Debug("Cookie file invalid (missing cookieValue).");
                    return false;
                }

                var apiUri = new Uri(BLConstants.BEATLEADER_API_URL);

                // Defaults for BeatLeader auth cookie
                const string defaultName = ".AspNetCore.Cookies";
                const string defaultPath = "/";
                const bool defaultSecure = true;
                const bool defaultHttpOnly = true;

                var cookie = new Cookie(defaultName, cookieInfo.cookieValue, defaultPath, apiUri.Host) {
                    Secure = defaultSecure,
                    HttpOnly = defaultHttpOnly
                };

                WebRequestFactory.CookieContainer.Add(new Uri($"{apiUri.Scheme}://{apiUri.Host}"), cookie);
                return true;
            } catch (Exception ex) {
                Plugin.Log.Error("TryCookieLogin exception:\n" + ex);
                return false;
            }
        }

        #endregion
    }
}