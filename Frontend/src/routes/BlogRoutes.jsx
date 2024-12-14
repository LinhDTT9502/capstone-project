import React from "react";
import { Routes, Route } from "react-router-dom";
import BlogList from "../pages/BlogList";
import BlogDetail from "../pages/BlogDetail";

const BlogRoutes = () => {
  return (
    <Routes>
      <Route path="/" element={<BlogList />} />
      <Route path="/:blogId" element={<BlogDetail />} />
    </Routes>
  );
};

export default BlogRoutes;