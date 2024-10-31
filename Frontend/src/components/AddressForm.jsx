import React, { useState, useEffect } from "react";
import { useTranslation } from "react-i18next";
import { fetchDistrict, fetchProvince, fetchWard } from "../services/GHN/GHNService";

const AddressForm = ({ onAddressChange }) => {
    const { t } = useTranslation();
    const [formData, setFormData] = useState({
        province: "",
        district: "",
        ward: "",
        street: ""
    });
    const [provinces, setProvinces] = useState([]);
    const [districts, setDistricts] = useState([]);
    const [wards, setWards] = useState([]);

    // Fetch provinces on mount
    useEffect(() => {
        const fetchProvinces = async () => {
            const result = await fetchProvince();
            setProvinces(result);
        };
        fetchProvinces();
    }, []);

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
            onAddressChange(addressString, formData.ward, formData.district);
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
        <div>
            <input
                type="text"
                name="street"
                value={formData.street}
                onChange={handleInputChange}
                className="mt-1 block w-full border rounded-md p-2 focus:ring-blue-500 focus:border-blue-500"
                placeholder="số nhà, tên đường. ví dụ 123 đường mạc đĩnh chi"
            />

            <select
                className="form-select form-select-sm mb-3"
                name="province"
                value={formData.province}
                onChange={handleInputChange}
            >
                <option value=""> chọn tỉnh thành</option>
                {provinces.map(province => (
                    <option key={province.ProvinceID} value={province.ProvinceID}>
                        {province.ProvinceName}
                    </option>
                ))}
            </select>

            <select
                className="form-select form-select-sm mb-3"
                name="district"
                value={formData.district}
                onChange={handleInputChange}
                disabled={!formData.province}
            >
                <option value="">{t("address_form.select_district")}</option>
                {districts.map(district => (
                    <option key={district.DistrictID} value={district.DistrictID}>
                        {district.DistrictName}
                    </option>
                ))}
            </select>

            <select
                className="form-select form-select-sm"
                name="ward"
                value={formData.ward}
                onChange={handleInputChange}
                disabled={!formData.district}
            >
                <option value="">{t("address_form.select_ward")}</option>
                {wards.map(ward => (
                    <option key={ward.WardCode} value={ward.WardCode}>
                        {ward.WardName}
                    </option>
                ))}
            </select>
        </div>
    );
};

export default AddressForm;
