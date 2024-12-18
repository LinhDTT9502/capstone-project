import React, { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { fetchBlogById } from "../services/blogService";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faArrowLeft } from "@fortawesome/free-solid-svg-icons";

const BlogDetail = () => {
  const navigate = useNavigate();
  const { blogId } = useParams();
  const [blog, setBlog] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const loadBlog = async () => {
      try {
        const fetchedBlog = await fetchBlogById(blogId);
        console.log(fetchedBlog);

        setBlog(fetchedBlog);
      } catch (error) {
        console.error("Error fetching blog by ID:", error);
      } finally {
        setLoading(false);
      }
    };

    loadBlog();
  }, [blogId]);

  if (loading) {
    return (
      <div className="bg-gray-100 min-h-screen flex items-center justify-center">
        <i className="fas fa-spinner fa-spin text-6xl text-blue-500"></i>
      </div>
    );
  }

  if (!blog) {
    return (
      <div className="bg-gray-100 min-h-screen flex flex-col items-center justify-center">
        <i className="fas fa-exclamation-circle text-6xl text-red-500 mb-4"></i>
        <p className="text-xl text-gray-700 mb-4">Hiện tại không có bài viết nào.</p>
        <button
          onClick={() => navigate(-1)}
          className="bg-blue-500 text-white px-6 py-2 rounded-full hover:bg-blue-600 transition duration-300 ease-in-out flex items-center"
        >
          <FontAwesomeIcon
            className="pr-2"
            icon={faArrowLeft}
          /> Quay lại
        </button>
      </div>
    );
  }

  return (
    <div className="bg-gray-100 min-h-screen">
      <div
        className="relative flex justify-center items-center h-96 bg-cover bg-center"
        style={{ backgroundImage: `url(${blog.coverImgPath || '/assets/images/default-cover.jpg'})` }}
      >
        <div className="absolute inset-0 bg-black opacity-50"></div>

        <h1 className="text-5xl font-bold text-white relative z-10 text-center px-4 animate-fade-in-down">
          {blog.title}
        </h1>
      </div>

      <div className="max-w-4xl mx-auto py-12 px-4">
        <div className="bg-white rounded-lg shadow-lg p-8 mb-8 animate-fade-in">
          <p className="text-2xl text-gray-700 mb-6 font-light italic">{blog.subTitle}</p>

          <div className="flex items-center mb-8 border-b pb-4">
            <div className="w-16 h-16 rounded-full bg-blue-500 flex items-center justify-center text-white text-2xl mr-4">
              <i className="fas fa-user"></i>
            </div>
            <div>
              <p className="text-xl font-medium text-gray-800">{blog.createdByStaffFullName}</p>
              <p className="text-sm text-gray-500">
                <i className="far fa-calendar-alt mr-2"></i>
                {new Date(blog.createAt).toLocaleDateString("en-US", {
                  year: "numeric",
                  month: "long",
                  day: "numeric",
                })}
              </p>
            </div>
          </div>


          <div
            className="prose prose-lg max-w-none text-gray-800 leading-relaxed"
            dangerouslySetInnerHTML={{ __html: blog.content }}
          ></div>

        </div>

        <div className="flex justify-between items-center animate-fade-in">
          <button
            onClick={() => navigate(-1)}
            className="bg-blue-500 text-white px-6 py-2 rounded-full hover:bg-blue-600 transition duration-300 ease-in-out flex items-center"
          >
            <FontAwesomeIcon
              className="pr-2"
              icon={faArrowLeft}
            /> Quay lại
          </button>
          <div className="flex space-x-4">
            <button className="text-blue-500 hover:text-blue-600 transition duration-300 ease-in-out">
              <i className="far fa-heart text-2xl"></i>
            </button>
            <button className="text-blue-500 hover:text-blue-600 transition duration-300 ease-in-out">
              <i className="far fa-comment text-2xl"></i>
            </button>
            <button className="text-blue-500 hover:text-blue-600 transition duration-300 ease-in-out">
              <i className="fas fa-share-alt text-2xl"></i>
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

export default BlogDetail;

