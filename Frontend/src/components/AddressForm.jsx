// AddressForm.jsx

import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { fetchDistrict, fetchProvince, fetchWard } from "../services/GHN/GHNService";
import { Input } from "@material-tailwind/react";

const AddressForm = ({ onAddressChange, province, district, ward, address }) => {
  const { t } = useTranslation();
  const [formData, setFormData] = useState({
    province: province || "",
    district: district || "",
    ward: ward || "",
    street: address || "",
  });
  const [provinces, setProvinces] = useState([]);
  const [districts, setDistricts] = useState([]);
  const [wards, setWards] = useState([]);

  const handleInputChange = (event) => {
    const { name, value } = event.target;
    setFormData((prev) => ({ ...prev, [name]: value }));

    if (name === "province") {
      fetchDistricts(value);
      setDistricts([]);
      setWards([]);
    } else if (name === "district") {
      fetchWards(value);
      setWards([]);
    }
  };

  // Fetch provinces on mount
  useEffect(() => {
    const fetchProvinces = async () => {
      const result = await fetchProvince();
      setProvinces(result);
    };
    fetchProvinces();
  }, []);

  const fetchDistricts = async (provinceId) => {
    const result = await fetchDistrict(provinceId);
    setDistricts(result);
  };

  const fetchWards = async (districtId) => {
    const result = await fetchWard(districtId);
    setWards(result);
  };

  // Watch for changes in province, district, and ward, and recompute the address
  useEffect(() => {
    if (formData.province && formData.district && formData.ward) {
      const addressString = getAddressString();
      onAddressChange(addressString); // Pass the full address to the parent
    }
  }, [formData]);

  const getAddressString = () => {
    const selectedWard = wards.find(w => w.WardCode === formData.ward);
    const selectedDistrict = districts.find(d => d.DistrictID === Number(formData.district));
    const selectedProvince = provinces.find(p => p.ProvinceID === Number(formData.province));

    const wardName = selectedWard ? selectedWard.WardName : "";
    const districtName = selectedDistrict ? selectedDistrict.DistrictName : "";
    const provinceName = selectedProvince ? selectedProvince.ProvinceName : "";

    return `${formData.street}, ${wardName}, ${districtName}, ${provinceName}`.trim();
  };

  return (
    <div className="space-y-2 bg-white ">
      <Input
        type="text"
        name="street"
        value={formData.street}
        onChange={handleInputChange}
        label="số nhà, tên đường. ví dụ 123 đường Mạc Đĩnh Chi"
        required
      />
      <div className="space-x-2 flex">
        <select
          className="p-2 rounded"
          name="province"
          value={formData.province}
          onChange={handleInputChange}
        >
          <option value="">Chọn tỉnh thành</option>
          {provinces.map((province) => (
            <option key={province.ProvinceID} value={province.ProvinceID}>
              {province.ProvinceName}
            </option>
          ))}
        </select>
        <select
          name="district"
          value={formData.district}
          onChange={handleInputChange}
          disabled={!formData.province}
        >
          <option value="">{t("address_form.select_district")}</option>
          {districts.map((district) => (
            <option key={district.DistrictID} value={district.DistrictID}>
              {district.DistrictName}
            </option>
          ))}
        </select>
        <select
          name="ward"
          value={formData.ward}
          onChange={handleInputChange}
          disabled={!formData.district}
        >
          <option value="">{t("address_form.select_ward")}</option>
          {wards.map((ward) => (
            <option key={ward.WardCode} value={ward.WardCode}>
              {ward.WardName}
            </option>
          ))}
        </select>
      </div>
    </div>
  );
};

export default AddressForm;
