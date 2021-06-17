using Newtonsoft.Json;
using System.Collections.Generic;

namespace LaPakguette.Lib.Models
{
    internal class Types
    {
        [JsonProperty("UnrealBuildTool.EncryptionAndSigning+CryptoSettings,UnrealBuildTool,Version4.0.0.0,Cultureneutral,internalKeyTokennull")]
        internal string UnrealBuildToolEncryptionAndSigningCryptoSettingsUnrealBuildToolVersion4000CultureneutralinternalKeyTokennull { get; set; }

        [JsonProperty("UnrealBuildTool.EncryptionAndSigning+EncryptionKey,UnrealBuildTool,Version4.0.0.0,Cultureneutral,internalKeyTokennull")]
        internal string UnrealBuildToolEncryptionAndSigningEncryptionKeyUnrealBuildToolVersion4000CultureneutralinternalKeyTokennull { get; set; }

        [JsonProperty("UnrealBuildTool.EncryptionAndSigning+SigningKeyPair,UnrealBuildTool,Version4.0.0.0,Cultureneutral,internalKeyTokennull")]
        internal string UnrealBuildToolEncryptionAndSigningSigningKeyPairUnrealBuildToolVersion4000CultureneutralinternalKeyTokennull { get; set; }

        [JsonProperty("UnrealBuildTool.EncryptionAndSigning+SigningKey,UnrealBuildTool,Version4.0.0.0,Cultureneutral,internalKeyTokennull")]
        internal string UnrealBuildToolEncryptionAndSigningSigningKeyUnrealBuildToolVersion4000CultureneutralinternalKeyTokennull { get; set; }
    }

    internal class EncryptionKey
    {
        [JsonProperty("$type")]
        internal string Type { get; set; }

        [JsonProperty("Name")]
        internal string Name { get; set; }

        [JsonProperty("Guid")]
        internal string Guid { get; set; }

        [JsonProperty("Key")]
        internal string Key { get; set; }
    }

    internal class CryptoModel
    {
        [JsonProperty("$types")]
        internal Types Types { get; set; }

        [JsonProperty("$type")]
        internal string Type { get; set; }

        [JsonProperty("EncryptionKey")]
        internal EncryptionKey EncryptionKey { get; set; }

        [JsonProperty("SigningKey")]
        internal object SigningKey { get; set; }

        [JsonProperty("bEnablePakSigning")]
        internal bool BEnablePakSigning { get; set; }

        [JsonProperty("bEnablePakIndexEncryption")]
        internal bool BEnablePakIndexEncryption { get; set; }

        [JsonProperty("bEnablePakIniEncryption")]
        internal bool BEnablePakIniEncryption { get; set; }

        [JsonProperty("bEnablePakUAssetEncryption")]
        internal bool BEnablePakUAssetEncryption { get; set; }

        [JsonProperty("bEnablePakFullAssetEncryption")]
        internal bool BEnablePakFullAssetEncryption { get; set; }

        [JsonProperty("bDataCryptoRequired")]
        internal bool BDataCryptoRequired { get; set; }

        [JsonProperty("PakEncryptionRequired")]
        internal bool PakEncryptionRequired { get; set; }

        [JsonProperty("PakSigningRequired")]
        internal bool PakSigningRequired { get; set; }

        [JsonProperty("SecondaryEncryptionKeys")]
        internal List<object> SecondaryEncryptionKeys { get; set; }
    }




}
