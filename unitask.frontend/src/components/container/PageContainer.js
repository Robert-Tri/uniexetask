import React from 'react';

//Được import để kiểm tra kiểu dữ liệu của các props truyền vào component 
//giúp bắt lỗi khi truyền sai kiểu dữ liệu.
import PropTypes from 'prop-types';

//Quản lý các thẻ <head> của trang, như <title> và các thẻ meta.
import { Helmet } from 'react-helmet';

const PageContainer = ({ title, description, children }) => (
  <div>
    <Helmet>
      <title>{title}</title>
      <meta name="description" content={description} />
    </Helmet>
    {children}
  </div>
);

PageContainer.propTypes = {
  title: PropTypes.string,
  description: PropTypes.string,
  children: PropTypes.node,
};

export default PageContainer;
