import React, { useState, useEffect } from 'react';
import { fetchCategories } from '../../services/categoryService';
import { fetchBrands } from '../../services/brandService';
import { fetchBranchs } from '../../services/branchService';
import { fetchSports } from '../../services/sportService';
import { fetchSuppliers } from '../../services/supplierService';

const ProductForm = () => {
  const [productName, setProductName] = useState('');
  const [description, setDescription] = useState('');
  const [category, setCategory] = useState('');
  const [brandName, setBrandName] = useState('');
  const [sku, setSku] = useState('');
  const [stockQuantity, setStockQuantity] = useState('');
  const [regularPrice, setRegularPrice] = useState('');
  const [salePrice, setSalePrice] = useState('');
  const [rentPrice, setRentPrice] = useState('');  // Add rent price
  const [isRent, setIsRent] = useState(false);     // Rent status
  const [tags, setTags] = useState(['Lorem', 'Lorem', 'Lorem']);
  const [gallery, setGallery] = useState([]);
  const [categories, setCategories] = useState([]);
  const [brands, setBrands] = useState([]);
  const [branch, setBranch] = useState([]);
  const [sport, setSport] = useState([]);
  const [supplier, setSupplier] = useState([]);
  const [branchId, setBranchId] = useState('');
  const [sportId, setSportId] = useState('');
  const [supplierId, setSupplierId] = useState('');
  const [size, setSize] = useState('');
  const [color, setColor] = useState('');
  const [lotCode, setLotCode] = useState('');
  const [imgAvatarName, setImgAvatarName] = useState('');

  const handleMainImageChange = (e) => {
    const file = e.target.files[0];
    setMainImage(file);  // Save the raw file
  };

  const handleProductImagesChange = (e) => {
    const files = Array.from(e.target.files);
    setProductImages(files);  // Save raw files for product images
  };

  useEffect(() => {
    const loadCategories = async () => {
      try {
        const fetchedCategories = await fetchCategories();
        setCategories(fetchedCategories);
      } catch (error) {
        console.error('Failed to fetch categories:', error);
      }
    };

    const loadBrands = async () => {
      try {
        const fetchedBrands = await fetchBrands();
        setBrands(fetchedBrands);
      } catch (error) {
        console.error('Failed to fetch brands:', error);
      }
    };

    const loadBranchs = async () => {
      try {
        const fetchedBranchs = await fetchBranchs();
        setBranch(fetchedBranchs);
      } catch (error) {
        console.error('Failed to fetch Branchs:', error);
      }
    };

    const loadSports = async () => {
      try {
        const fetchedSports = await fetchSports();
        setSport(fetchedSports);
      } catch (error) {
        console.error('Failed to fetch Sport:', error);
      }
    };

    const loadSuppliers = async () => {
      try {
        const fetchedSuppliers = await fetchSuppliers();
        setSupplier(fetchedSuppliers);
      } catch (error) {
        console.error('Failed to fetch Supplier:', error);
      }
    };

    loadCategories();
    loadBrands();
    loadBranchs();
    loadSports();
    loadSuppliers();
  }, []);

  const handleImport = async () => {
    const token = localStorage.getItem('token');
    console.log(branch, sport, supplier);

    const formData = new FormData();
    formData.append('ProductName', productName);
    formData.append('Description', description);
    formData.append('CategoryId', category);
    formData.append('BrandId', brandName);
    formData.append('BranchId', branchId);
    formData.append('SportId', sportId);
    formData.append('SupplierId', supplierId);
    formData.append('ProductCode', sku);
    formData.append('LotCode', lotCode);
    formData.append('Color', color);
    formData.append('Size', size);
    formData.append('Quantity', stockQuantity);
    formData.append('ListedPrice', regularPrice);
    formData.append('Price', salePrice);
    formData.append('RentPrice', rentPrice);
    formData.append('IsRent', isRent);
    formData.append('ImgAvatarName', imgAvatarName);
    formData.append('MainImage', gallery[0]); // Assuming the first file is the main image
    gallery.forEach((image, index) => formData.append(`ProductImages`, image));
    for (let pair of formData.entries()) {
      console.log(`${pair[0]}: ${pair[1]}`);
    }


    try {
      const response = await fetch('https://capstone-project-703387227873.asia-southeast1.run.app/api/Product/import-product', {
        method: 'POST',
        headers: {
          Authorization: `Bearer ${token}`,
        },
        body: formData,
      });

      if (response.ok) {
        alert('Product imported successfully!');
      } else {
        alert('Failed to import product');
      }
    } catch (error) {
      console.error('Error importing product:', error);
      alert('Error importing product');
    }
  };

  return (
    <div className="bg-white p-6 rounded-lg shadow-md w-full max-w-4xl mx-auto">
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <div>
          <label className="block text-sm font-medium text-gray-700">Product Name</label>
          <input
            type="text"
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={productName}
            onChange={(e) => setProductName(e.target.value)}
            placeholder="Type name here"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">Supplier</label>
          <select
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={supplierId}
            onChange={(e) => setSupplierId(e.target.value)}
          >
            <option value="" disabled>Select a supplier</option>
            {supplier.map((supplier) => (
              <option key={supplier.id} value={supplier.id}>{supplier.supplierName}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">Branch</label>
          <select
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={branchId}
            onChange={(e) => setBranchId(e.target.value)} // Store only the selected branch ID
          >
            <option value="" disabled>Select a branch</option>
            {branch.map((branch) => (
              <option key={branch.id} value={branch.id}>{branch.branchName}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">Sport</label>
          <select
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={sportId}
            onChange={(e) => setSportId(e.target.value)}
          >
            <option value="" disabled>Select a sport</option>
            {sport.map((sport) => (
              <option key={sport.id} value={sport.id}>{sport.name}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">Category</label>
          <select
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={category}
            onChange={(e) => setCategory(e.target.value)}
          >
            <option value="" disabled>Select a category</option>
            {categories.map((cat) => (
              <option key={cat.id} value={cat.id}>{cat.categoryName}</option>
            ))}
          </select>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">Brand Name</label>
          <select
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={brandName}
            onChange={(e) => setBrandName(e.target.value)}
          >
            <option value="" disabled>Select a brand</option>
            {brands.map((brand) => (
              <option key={brand.id} value={brand.id}>{brand.brandName}</option>
            ))}
          </select>
        </div>
        <div className="grid grid-cols-3 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700">ProductCode</label>
            <input
              type="text"
              className="mt-1 p-2 border border-gray-300 rounded w-full"
              value={sku}
              onChange={(e) => setSku(e.target.value)}
              placeholder="Fox-3983"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700">LotCode</label>
            <input
              type="text"
              className="mt-1 p-2 border border-gray-300 rounded w-full"
              value={lotCode}
              onChange={(e) => setLotCode(e.target.value)}
              placeholder="Fox-3983"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700"> Quantity</label>
            <input
              type="text"
              className="mt-1 p-2 border border-gray-300 rounded w-full"
              value={stockQuantity}
              onChange={(e) => setStockQuantity(e.target.value)}
              placeholder="1258"
            />
          </div>
        </div>
        <div className="grid grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium text-gray-700">Listed Price</label>
            <input
              type="text"
              className="mt-1 p-2 border border-gray-300 rounded w-full"
              value={regularPrice}
              onChange={(e) => setRegularPrice(e.target.value)}
              placeholder="$1000"
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700"> Price</label>
            <input
              type="text"
              className="mt-1 p-2 border border-gray-300 rounded w-full"
              value={salePrice}
              onChange={(e) => setSalePrice(e.target.value)}
              placeholder="$450"
            />
          </div>
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">Rent Status</label>
          <select
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={isRent}
            onChange={(e) => setIsRent(e.target.value === 'true')}
          >
            <option value="false">No</option>
            <option value="true">Yes</option>
          </select>
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Rent Price</label>
          <input
            type="text"
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={rentPrice}
            onChange={(e) => setRentPrice(e.target.value)}
            placeholder="Rent Price"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">Description</label>
          <textarea
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            placeholder="Type Description here"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">Size</label>
          <input
            type="text"
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={size}
            onChange={(e) => setSize(e.target.value)}
            placeholder="Type size here"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">Color</label>
          <input
            type="text"
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={color}
            onChange={(e) => setColor(e.target.value)}
            placeholder="Type size here"
          />
        </div>
        <div>
          <label className="block text-sm font-medium text-gray-700">ImgAvatarName</label>
          <input
            type="text"
            className="mt-1 p-2 border border-gray-300 rounded w-full"
            value={imgAvatarName}
            onChange={(e) => setImgAvatarName(e.target.value)}
            placeholder="Type size here"
          />
        </div>

      </div>

      <div className="mt-6 grid grid-cols-1 md:grid-cols-2 gap-4">

        {/* Main Image Upload */}
        <div>
          <label className="block text-sm font-medium text-gray-700">Main Image</label>
          <input
            type="file"
            onChange={handleMainImageChange}  // Handle main image selection
            accept="image/*"
            className="mt-2"
          />
        </div>

        {/* Product Gallery Upload */}
        <div className="flex flex-col items-center justify-center border-2 border-dashed border-gray-300 rounded-lg p-6">
          <label className="block text-sm font-medium text-gray-700 mb-4">Product Gallery</label>
          {/* <input
            type="file"
            multiple
            className="hidden"
            id="file-upload"
            onChange={handleFileChange} // Handle multiple images upload
          /> */}
          <label htmlFor="file-upload" className="cursor-pointer">
            <div className="flex flex-col items-center">
              <img src="path/to/placeholder-image.png" alt="Upload" className="w-20 h-20 object-cover mb-4" />
              <p className="text-gray-500">Drop your image here, or browse</p>
              <p className="text-gray-400 text-sm">Jpeg, png are allowed</p>
            </div>
          </label>
        </div>

        {/* Product Gallery Preview and Delete Functionality */}
        <div>
          <label className="block text-sm font-medium text-gray-700">Product Gallery</label>
          <input
            type="file"
            accept="image/*"
            multiple
            className="mt-2"
            onChange={(e) => {
              const files = Array.from(e.target.files);
              setGallery([...gallery, ...files]); // Append new images to the gallery
            }}
          />
          <ul className="mt-2 space-y-2">
            {gallery.map((src, index) => (
              <li key={index} className="flex items-center">
                <img src={URL.createObjectURL(src)} alt={`Product thumbnail ${index}`} className="w-12 h-12 object-cover rounded-lg" />
                <span className="ml-3 text-gray-700">Product thumbnail.png</span>
                <button
                  type="button"
                  className="ml-auto text-red-600 hover:text-red-900"
                  onClick={() => setGallery(gallery.filter((_, i) => i !== index))}
                >
                  Delete
                </button>
              </li>
            ))}
          </ul>
        </div>

      </div>

      <div className="mt-6 flex justify-between">
        <button
          type="button"
          className="bg-red-600 text-white px-4 py-2 rounded-lg shadow hover:bg-red-700"
        >
          CANCEL
        </button>
        <button
          type="button"
          className="bg-blue-300 text-white px-4 py-2 rounded-lg shadow hover:bg-blue-400"
          onClick={handleImport}
        >
          IMPORT
        </button>
      </div>
    </div>
  );
};

export default ProductForm;
