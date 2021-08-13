using System.Collections.Generic;
using Newtonsoft.Json;

namespace LaPakguette.Lib.Models
{
    public class Types
    {
        [JsonProperty(
            "UnrealBuildTool.EncryptionAndSigning+CryptoSettings,UnrealBuildTool,Version4.0.0.0,Cultureneutral,publicKeyTokennull")]
        public string
            UnrealBuildToolEncryptionAndSigningCryptoSettingsUnrealBuildToolVersion4000CultureneutralpublicKeyTokennull
        {
            get;
            set;
        }

        [JsonProperty(
            "UnrealBuildTool.EncryptionAndSigning+EncryptionKey,UnrealBuildTool,Version4.0.0.0,Cultureneutral,publicKeyTokennull")]
        public string
            UnrealBuildToolEncryptionAndSigningEncryptionKeyUnrealBuildToolVersion4000CultureneutralpublicKeyTokennull
        {
            get;
            set;
        }

        [JsonProperty(
            "UnrealBuildTool.EncryptionAndSigning+SigningKeyPair,UnrealBuildTool,Version4.0.0.0,Cultureneutral,publicKeyTokennull")]
        public string
            UnrealBuildToolEncryptionAndSigningSigningKeyPairUnrealBuildToolVersion4000CultureneutralpublicKeyTokennull
        {
            get;
            set;
        }

        [JsonProperty(
            "UnrealBuildTool.EncryptionAndSigning+SigningKey,UnrealBuildTool,Version4.0.0.0,Cultureneutral,publicKeyTokennull")]
        public string
            UnrealBuildToolEncryptionAndSigningSigningKeyUnrealBuildToolVersion4000CultureneutralpublicKeyTokennull
        {
            get;
            set;
        }
    }

    public class EncryptionKey
    {
        [JsonProperty("$type")] public string Type { get; set; }

        [JsonProperty("Name")] public string Name { get; set; }

        [JsonProperty("Guid")] public string Guid { get; set; }

        [JsonProperty("Key")] public string Key { get; set; }
    }

    public class CryptoModel
    {
        [JsonProperty("$types")] public Types Types { get; set; }

        [JsonProperty("$type")] public string Type { get; set; }

        [JsonProperty("EncryptionKey")] public EncryptionKey EncryptionKey { get; set; }

        [JsonProperty("SigningKey")] public object SigningKey { get; set; }

        [JsonProperty("bEnablePakSigning")] public bool BEnablePakSigning { get; set; }

        [JsonProperty("bEnablePakIndexEncryption")]
        public bool BEnablePakIndexEncryption { get; set; }

        [JsonProperty("bEnablePakIniEncryption")]
        public bool BEnablePakIniEncryption { get; set; }

        [JsonProperty("bEnablePakUAssetEncryption")]
        public bool BEnablePakUAssetEncryption { get; set; }

        [JsonProperty("bEnablePakFullAssetEncryption")]
        public bool BEnablePakFullAssetEncryption { get; set; }

        [JsonProperty("bDataCryptoRequired")] public bool BDataCryptoRequired { get; set; }

        [JsonProperty("PakEncryptionRequired")]
        public bool PakEncryptionRequired { get; set; }

        [JsonProperty("PakSigningRequired")] public bool PakSigningRequired { get; set; }

        [JsonProperty("SecondaryEncryptionKeys")]
        public List<object> SecondaryEncryptionKeys { get; set; }
    }
}