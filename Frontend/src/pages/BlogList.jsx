import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { fetchAllBlogs } from "../services/blogService";

const BlogCard = ({ blog, onClick }) => {
  const [isHovered, setIsHovered] = useState(false);

  return (
    <div
      className="bg-white rounded-lg shadow-md overflow-hidden cursor-pointer transition-all duration-300 ease-in-out transform hover:shadow-xl hover:-translate-y-2"
      onClick={onClick}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      <div className="relative overflow-hidden">
        {blog.coverImgPath && (
          <img
            src={blog.coverImgPath}
            alt="Blog Cover"
            className={`w-full h-48 object-contain transition-transform duration-300 ease-in-out ${
              isHovered ? 'scale-110' : 'scale-100'
            }`}
          />
        )}
        <div className={`absolute inset-0 bg-black bg-opacity-50 flex items-center justify-center transition-opacity duration-300 ${
          isHovered ? 'opacity-100' : 'opacity-0'
        }`}>
          <span className="text-white text-lg font-semibold">Đọc thêm</span>
        </div>
      </div>
      <div className="p-6">
        <h2 className="text-xl font-bold text-gray-800 line-clamp-2 mb-2">
          {blog.title}
        </h2>
        <p className="text-sm text-gray-600 line-clamp-2 mb-4">
          {blog.subTitle}
        </p>
        <div className="flex items-center">
          <div className="w-10 h-10 rounded-full bg-blue-500 flex items-center justify-center text-white mr-4">
          <img
            src={"/assets/images/staff-avatar.png"}
            alt="Blog Cover"
          />
          </div>
          <div>
            <p className="text-sm font-medium text-gray-700">
              {blog.createdByStaffFullName}
            </p>
            <p className="text-xs text-gray-500">
              {new Date(blog.createAt).toLocaleDateString('en-US', {
                year: 'numeric',
                month: 'long',
                day: 'numeric'
              })}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
};

const SkeletonLoader = () => (
  <div className="bg-white rounded-lg shadow-md overflow-hidden animate-pulse">
    <div className="w-full h-48 bg-gray-300"></div>
    <div className="p-6">
      <div className="h-6 bg-gray-300 rounded w-3/4 mb-2"></div>
      <div className="h-4 bg-gray-300 rounded w-full mb-4"></div>
      <div className="flex items-center">
        <div className="rounded-full bg-gray-300 h-10 w-10 mr-4"></div>
        <div>
          <div className="h-4 bg-gray-300 rounded w-20 mb-1"></div>
          <div className="h-3 bg-gray-300 rounded w-24"></div>
        </div>
      </div>
    </div>
  </div>
);

const BlogList = () => {
  const navigate = useNavigate();
  const [blogs, setBlogs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState("");

  useEffect(() => {
    const loadBlogs = async () => {
      try {
        const blogData = await fetchAllBlogs();
        setBlogs(blogData);
      } catch (error) {
        console.error("Error fetching blogs:", error);
      } finally {
        setLoading(false);
      }
    };

    loadBlogs();
  }, []);

  const filteredBlogs = blogs.filter(blog =>
    blog.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
    blog.subTitle.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const renderBlogItem = (blog) => (
    <BlogCard
      key={blog.blogId}
      blog={blog}
      onClick={() => navigate(`/blog/${blog.blogId}`)}
    />
  );
  

  return (
    <div className="bg-gray-100 min-h-screen">
      <div
        className="relative flex flex-col justify-center items-center h-[400px]"
        style={{ 
          backgroundImage: "url('/assets/images/blog/cover_image.jpg')", 
          backgroundSize: "cover", 
          backgroundPosition: "center" 
        }}
      >
        <div className="absolute inset-0 bg-black opacity-60"></div>
        <h1 className="text-[40px] text-white font-alfa relative z-10 mb-6 animate-fade-in-down">Blog Feed</h1>
        <div className="relative z-10 w-full max-w-md px-4">
          <input
            type="text"
            placeholder="Tìm kiếm..."
            className="w-full px-4 py-2 rounded-full border-2 border-white bg-transparent text-white placeholder-gray-300 focus:outline-none focus:border-blue-500 transition-all duration-300"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
          <i className="fas fa-search absolute right-8 top-3 text-white"></i>
        </div>
      </div>

      <div className="max-w-7xl mx-auto py-10 px-4">
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
          {loading
            ? Array.from({ length: 6 }).map((_, index) => (
                <SkeletonLoader key={index} />
              ))
            : filteredBlogs.map((blog) => renderBlogItem(blog))}
        </div>
        {!loading && filteredBlogs.length === 0 && (
          <div className="text-center text-gray-600 mt-10">
            <i className="fas fa-search fa-3x mb-4"></i>
            <p className="text-xl">No blogs found matching your search.</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default BlogList;

