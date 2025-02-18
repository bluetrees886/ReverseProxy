using HttpProxyAuthentication.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace HttpProxyAuthentication
{
    public static class PermissionExtensions
    {
        public static PermissionVerifyState Verify(ref this Permission permission, ISigner signer, Func<string, string, string> verifyKey, DateTime now, int timePhase)
        {
            if (string.IsNullOrEmpty(permission.User))
                return PermissionVerifyState.UserEmpty;

            if (permission.Time == null)
                return PermissionVerifyState.TimeFlaw;

            var time = permission.Time.Value;
            if (Math.Abs((time - now).TotalSeconds) < timePhase)
                return PermissionVerifyState.TimeDiff;

            if (string.IsNullOrEmpty(permission.Signature))
                return PermissionVerifyState.SignEmpty;

            var keyStr = verifyKey(permission.User, permission.Version);
            if (keyStr == null)
                return PermissionVerifyState.UserUnknown;

            if (signer.Verify(permission.User, permission.SignatureMessage, permission.Signature, keyStr))
            {
                return PermissionVerifyState.Success;
            }
            return PermissionVerifyState.VerifyError;
        }
        public static PermissionVerifyState Verify(ref this Permission permission, ISignerProvider signerProvider, Func<string, string, string> verifyKey, DateTime now, int timePhase)
        {
            if (string.IsNullOrEmpty(permission.User))
                return PermissionVerifyState.UserEmpty;

            if (permission.Time == null)
                return PermissionVerifyState.TimeFlaw;

            var time = permission.Time.Value;
            if (Math.Abs((time - now).TotalSeconds) < timePhase)
                return PermissionVerifyState.TimeDiff;

            if (string.IsNullOrEmpty(permission.Signature))
                return PermissionVerifyState.SignEmpty;

            ISigner signer;
            if (!signerProvider.TryGetSigner(permission.Version, out signer))
                return PermissionVerifyState.SignerUnknown;

            var keyStr = verifyKey(permission.User, permission.Version);
            if (keyStr == null)
                return PermissionVerifyState.UserUnknown;

            if (signer.Verify(permission.User, permission.SignatureMessage, permission.Signature, keyStr))
            {
                return PermissionVerifyState.Success;
            }
            return PermissionVerifyState.VerifyError;
        }
        public static string Sign(ref this Permission permission, ISigner signer, string key, string keyPass)
        {
            return permission.Signature = signer.Sign(permission.User, permission.SignatureMessage, key, keyPass);
        }
        public static string Sign(ref this Permission permission, ISignerProvider signerProvider, string key, string keyPass)
        {
            if (signerProvider.TryGetSigner(permission.Version, out ISigner signer))
                return permission.Signature = signer.Sign(permission.User, permission.SignatureMessage, key, keyPass);
            return default;
        }
    }
}
