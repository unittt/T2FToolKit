using System.Collections.Generic;
using UnityEngine;

namespace T2FToolKit
{
    internal static class LanguageUtil
    {
        private static readonly Dictionary<SystemLanguage, string> _languageMap = new()
        {
            { SystemLanguage.Afrikaans, "af_ZA" },
            { SystemLanguage.Arabic, "ar_AE" },
            { SystemLanguage.Basque, "eu_ES" },
            { SystemLanguage.Belarusian, "be_BY" },
            { SystemLanguage.Bulgarian, "bg_BG" },
            { SystemLanguage.Catalan, "ca_ES" },
            { SystemLanguage.Chinese, "zh_CN" },
            { SystemLanguage.ChineseSimplified, "zh_CN" },
            { SystemLanguage.ChineseTraditional, "zh_TW" },
            { SystemLanguage.Czech, "cs_CZ" },
            { SystemLanguage.Danish, "da_DK" },
            { SystemLanguage.Dutch, "nl_NL" },
            { SystemLanguage.English, "en_US" },
            { SystemLanguage.Estonian, "et_EE" },
            { SystemLanguage.Faroese, "fo_FO" },
            { SystemLanguage.Finnish, "fi_FI" },
            { SystemLanguage.French, "fr_FR" },
            { SystemLanguage.German, "de_DE" },
            { SystemLanguage.Greek, "el_GR" },
            { SystemLanguage.Hebrew, "he_IL" },
            { SystemLanguage.Icelandic, "is_IS" },
            { SystemLanguage.Indonesian, "id_ID" },
            { SystemLanguage.Italian, "it_IT" },
            { SystemLanguage.Japanese, "ja_JP" },
            { SystemLanguage.Korean, "ko_KR" },
            { SystemLanguage.Latvian, "lv_LV" },
            { SystemLanguage.Lithuanian, "lt_LT" },
            { SystemLanguage.Norwegian, "no_NO" },
            { SystemLanguage.Polish, "pl_PL" },
            { SystemLanguage.Portuguese, "pt_PT" },
            { SystemLanguage.Romanian, "ro_RO" },
            { SystemLanguage.Russian, "ru_RU" },
            { SystemLanguage.SerboCroatian, "sr_SP" },
            { SystemLanguage.Slovak, "sk_SK" },
            { SystemLanguage.Slovenian, "sl_SI" },
            { SystemLanguage.Spanish, "es_ES" },
            { SystemLanguage.Swedish, "sv_SE" },
            { SystemLanguage.Thai, "th_TH" },
            { SystemLanguage.Turkish, "tr_TR" },
            { SystemLanguage.Ukrainian, "uk_UA" },
            { SystemLanguage.Vietnamese, "vi_VN" },
            // ... 其他语言
        };


        public static string ToLocale(SystemLanguage language)
        {
            return _languageMap.TryGetValue(language, out var locale) ? locale : "en_US";
        }
    }
    
    
    /// <summary>
    /// 语言词条
    /// </summary>
    public struct LanguageEntry
    {
        public string Chinese;
        public string English;
        public string Portugal;
        public string Spanish;

        public LanguageEntry(string chinese,string english, string portugal, string spanish)
        {
            Chinese = chinese;
            English = english;
            Portugal = portugal;
            Spanish = spanish;
        }
        
        public string GetValue()
        {
            SystemLanguage systemLanguage = Application.systemLanguage;
            switch (systemLanguage)
            {
                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    return Chinese;
                case SystemLanguage.Portuguese:
                    return Portugal;
                case SystemLanguage.Spanish:
                    return Spanish;
                default:
                    return English;
            }
        }
    }
}