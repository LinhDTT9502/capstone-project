import { useEffect, useState } from "react";
import {
  Card,
  Breadcrumbs,
  CardBody,
  Typography,
  Checkbox,
  Button, // Import Button from Material Tailwind or any other UI library you are using
} from "@material-tailwind/react";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faMagnifyingGlass } from "@fortawesome/free-solid-svg-icons";
import { useTranslation } from "react-i18next";
import { fetchWarehouse } from "../../services/warehouseService";
import { fetchImport } from "../../services/ImportService";
import { toast } from "react-toastify";
import { useNavigate } from "react-router-dom"; // Import useNavigate

export default function Warehouse() {
  const { t } = useTranslation();
  const navigate = useNavigate(); // Initialize useNavigate
  const [imports, setImports] = useState([]);
  const [selectedRowKeys, setSelectedRowKeys] = useState([]);
  const [searchQuery, setSearchQuery] = useState("");

  useEffect(() => {
    const fetchData = async () => {
      try {
        const importData = await fetchWarehouse();
        setImports(importData);
      } catch (error) {
        console.error("Error fetching data:", error);
        setImports([]);
      }
    };

    fetchData();
  }, [t]);

  const onSelectChange = (selectedKey) => {
    setSelectedRowKeys((prevSelectedRowKeys) =>
      prevSelectedRowKeys.includes(selectedKey)
        ? prevSelectedRowKeys.filter((key) => key !== selectedKey)
        : [...prevSelectedRowKeys, selectedKey]
    );
  };

  const handleSearch = (e) => {
    setSearchQuery(e.target.value);
  };

  const filteredImports = imports.filter((importItem) =>
    importItem.productName && importItem.productName.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const handleQuantityChange = async (item, newQuantity) => {
    const oldQuantity = item.quantity;

    if (!newQuantity || newQuantity <= oldQuantity) {
      alert(`The entered quantity must be greater than the current quantity of ${oldQuantity}.`);
      return;
    }

    const quantityDifference = newQuantity - oldQuantity;
    try {
      // Call fetchImport to update quantity
      const response = await fetchImport(quantityDifference, item.productId, item.supplierId);

      // Update state after successful API call
      const updatedImports = imports.map((importItem) =>
        importItem.id === item.id ? { ...importItem, quantity: newQuantity } : importItem
      );
      setImports(updatedImports);
      toast.success("Import successful");
      console.log("Quantity updated successfully:", response);
    } catch (error) {
      console.error("Error updating quantity:", error);
    }
  };

  const handleAddProductClick = () => {
    navigate("/productform"); // Route to ProductForm page
  };

  return (
    <>
      <h2 className="text-2xl font-bold mx-10 mt-4">{t("dashboard.dashboard")}</h2>
      <div className="flex justify-between items-center mx-10 my-4">
        <Breadcrumbs className="flex-grow">
          <a href="#" className="opacity-60">
            {t("dashboard.home")}
          </a>
          <a href="#">{t("sidebar_staff.import")}</a>
        </Breadcrumbs>
        <Button
          color="orange"
          onClick={handleAddProductClick}
          className="rounded-full"
        >
          {t("Import.AddProduct")}
        </Button>
      </div>

      <Card className="h-full w-[95.7%] mx-10 my-10">
        <div className="flex flex-row justify-between">
          <Typography variant="h6" color="black" className="mx-10 mt-4 text-2xl">
            {t("Import.Details")}
          </Typography>
          <div className="flex w-[40%] bg-white border-2 border-orange-500 rounded-full p-2 mx-auto mr-20 h-[40%]">
            <input
              className="flex-grow bg-transparent outline-none placeholder-gray-400"
              placeholder={t("Import.Search")}
              type="text"
              value={searchQuery}
              onChange={handleSearch}
            />
            <FontAwesomeIcon icon={faMagnifyingGlass} className="items-center text-orange-500 font-medium pr-3" />
          </div>
        </div>

        <CardBody className="overflow-scroll px-0">
          <table className="w-full min-w-max table-auto text-left">
            <thead>
              <tr>
                <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                  <Checkbox
                    color="blue"
                    onChange={(e) => {
                      if (e.target.checked) {
                        setSelectedRowKeys(filteredImports.map((row) => row.id));
                      } else {
                        setSelectedRowKeys([]);
                      }
                    }}
                    checked={selectedRowKeys.length === filteredImports.length}
                    indeterminate={
                      selectedRowKeys.length > 0 &&
                      selectedRowKeys.length < filteredImports.length
                    }
                  />
                </th>
                <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                  <Typography
                    variant="large"
                    color="blue-gray"
                    className="font-normal leading-none opacity-70"
                  >
                    {t("Import.Id")}
                  </Typography>
                </th>
                <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                  <Typography
                    variant="large"
                    color="blue-gray"
                    className="font-normal leading-none opacity-70"
                  >
                    {t("Import.ProductName")}
                  </Typography>
                </th>
                <th className="border-y border-blue-gray-100 bg-blue-gray-50/50 p-4">
                  <Typography
                    variant="large"
                    color="blue-gray"
                    className="font-normal leading-none opacity-70"
                  >
                    {t("Import.Quantity")}
                  </Typography>
                </th>
              </tr>
            </thead>
            <tbody>
              {filteredImports.map((importItem) => (
                <tr key={importItem.id}>
                  <td className="border-y border-blue-gray-100 p-4">
                    <Checkbox
                      color="blue"
                      checked={selectedRowKeys.includes(importItem.id)}
                      onChange={() => onSelectChange(importItem.id)}
                    />
                  </td>
                  <td className="border-y border-blue-gray-100 p-4">
                    <Typography variant="body" color="black">
                      {importItem.id}
                    </Typography>
                  </td>
                  <td className="border-y border-blue-gray-100 p-4">
                    <Typography variant="body" color="black">
                      {importItem.productName}
                    </Typography>
                  </td>
                  <td className="border-y border-blue-gray-100 p-4">
                    <div className="w-2/12 text-center flex items-center justify-center">
                      <input
                        type="number"
                        className="w-12 mx-2 text-center"
                        value={importItem.quantity}
                        onChange={(e) =>
                          handleQuantityChange(importItem, parseInt(e.target.value))
                        }
                        min={importItem.quantity + 1}
                      />
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </CardBody>
      </Card>
    </>
  );
}
