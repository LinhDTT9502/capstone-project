import axios from 'axios';
import axiosInstance from './axiosInstance';

const API_BASE_URL = 'https://capstone-project-703387227873.asia-southeast1.run.app/api/Blog';

// Tạo một blog mới
export const createBlog = (blogData) => {
  const url = `${API_BASE_URL}/create-blog`;
  return axiosInstance.post(url, blogData, {
    headers: {
      'Content-Type': 'application/json',
      'accept': '*/*'
    }
  });
};

// Lấy tất cả các blog
export const getAllBlogs = () => {
  const url = `${API_BASE_URL}/get-all-blogs`;
  return axios.get(url, {
    headers: {
      'accept': '*/*'
    }
  });
};

// Lấy blog theo ID
export const getBlogById = (blogId) => {
  const url = `${API_BASE_URL}/get-blog-by-id`;
  return axios.get(`${url}?id=${blogId}`, {
    headers: {
      'accept': '*/*'
    }
  });
};

// Chỉnh sửa blog theo ID
export const editBlog = (blogId, updatedData) => {
  const url = `${API_BASE_URL}/edit-blog/${blogId}`;
  return axiosInstance.put(url, updatedData, {
    headers: {
      'Content-Type': 'application/json',
      'accept': '*/*'
    }
  });
};

// Hiển thị/ẩn blog theo ID
export const toggleBlogVisibility = (blogId) => {
  const url = `${API_BASE_URL}/hide-show-blog/${blogId}`;
  return axios.put(url, null, {
    headers: {
      'accept': '*/*'
    }
  });
};

// Xóa blog theo ID
export const deleteBlog = (blogId) => {
  const url = `${API_BASE_URL}/delete-blog/${blogId}`;
  return axiosInstance.delete(url, {
    headers: {
      'accept': '*/*'
    }
  });
};
