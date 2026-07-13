import i18n from "i18next";
import { initReactI18next } from "react-i18next";

import enCommon from "./locales/en/common.json";
import enNav from "./locales/en/nav.json";
import enFooter from "./locales/en/footer.json";
import enHome from "./locales/en/home.json";
import enAbout from "./locales/en/about.json";
import enActivities from "./locales/en/activities.json";
import enEvents from "./locales/en/events.json";
import enGallery from "./locales/en/gallery.json";
import enBlogs from "./locales/en/blogs.json";
import enVolunteer from "./locales/en/volunteer.json";
import enDonation from "./locales/en/donation.json";
import enContact from "./locales/en/contact.json";
import enValidation from "./locales/en/validation.json";

import taCommon from "./locales/ta/common.json";
import taNav from "./locales/ta/nav.json";
import taFooter from "./locales/ta/footer.json";
import taHome from "./locales/ta/home.json";
import taAbout from "./locales/ta/about.json";
import taActivities from "./locales/ta/activities.json";
import taEvents from "./locales/ta/events.json";
import taGallery from "./locales/ta/gallery.json";
import taBlogs from "./locales/ta/blogs.json";
import taVolunteer from "./locales/ta/volunteer.json";
import taDonation from "./locales/ta/donation.json";
import taContact from "./locales/ta/contact.json";
import taValidation from "./locales/ta/validation.json";

export const SUPPORTED_LANGUAGES = ["en", "ta"] as const;
export type SupportedLanguage = (typeof SUPPORTED_LANGUAGES)[number];

export const LANGUAGE_STORAGE_KEY = "trust-ngo-language";

const storedLanguage = typeof window !== "undefined" ? window.localStorage.getItem(LANGUAGE_STORAGE_KEY) : null;
const initialLanguage: SupportedLanguage =
  storedLanguage === "en" || storedLanguage === "ta" ? storedLanguage : "en";

i18n.use(initReactI18next).init({
  resources: {
    en: {
      common: enCommon,
      nav: enNav,
      footer: enFooter,
      home: enHome,
      about: enAbout,
      activities: enActivities,
      events: enEvents,
      gallery: enGallery,
      blogs: enBlogs,
      volunteer: enVolunteer,
      donation: enDonation,
      contact: enContact,
      validation: enValidation,
    },
    ta: {
      common: taCommon,
      nav: taNav,
      footer: taFooter,
      home: taHome,
      about: taAbout,
      activities: taActivities,
      events: taEvents,
      gallery: taGallery,
      blogs: taBlogs,
      volunteer: taVolunteer,
      donation: taDonation,
      contact: taContact,
      validation: taValidation,
    },
  },
  lng: initialLanguage,
  fallbackLng: "en",
  defaultNS: "common",
  ns: [
    "common",
    "nav",
    "footer",
    "home",
    "about",
    "activities",
    "events",
    "gallery",
    "blogs",
    "volunteer",
    "donation",
    "contact",
    "validation",
  ],
  interpolation: {
    escapeValue: false,
  },
  returnObjects: true,
});

export function changeLanguage(language: SupportedLanguage) {
  i18n.changeLanguage(language);
  window.localStorage.setItem(LANGUAGE_STORAGE_KEY, language);
}

export default i18n;
