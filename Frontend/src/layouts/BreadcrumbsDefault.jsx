import { Breadcrumbs } from "@material-tailwind/react";
import { useTranslation } from "react-i18next";
 
export function BreadcrumbsDefault() {
    const { t } = useTranslation();
  return (
    <div className="bg-white">
    <Breadcrumbs>
      <a href="#" className="opacity-60 text-black">
        {t("breadcrumbs.docs")}
      </a>
      <a href="#" className="opacity-60 text-black">
        {t("breadcrumbs.components")}
      </a>
      <a href="#">{t("breadcrumbs.breadcrumbs")}</a>
    </Breadcrumbs>
    </div>
  );
}