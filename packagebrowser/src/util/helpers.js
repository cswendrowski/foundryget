import { getByTag } from "locale-codes";
import { where } from "langs";

export const func = {
  getLanguageByTag: (language) => {
    // exceptions
    if (["cn", "zh-SHAN", "zh-HANS"].indexOf(language?.lang) !== -1)
      return "中文";
    if (language?.lang === "kr") return "한국어";

    // auto detect
    let lang = "";

    lang = where("1", language?.lang);
    if (lang) return lang.local;

    lang = where("2", language?.lang);
    if (lang) return lang.local;

    lang = getByTag(language?.lang)?.local;
    if (lang) return lang;

    lang = getByTag(language?.lang)?.name;
    if (lang) return lang;

    lang = language?.name;
    if (lang) {
      console.error(lang);
      return lang;
    }

    lang = language?.lang;
    return lang;
  },

  ciIncludes: (value, query) => {
    if (query.length === 0) { return true; }
    return value.toString().toLowerCase().includes(query.toLowerCase());
  },

};
