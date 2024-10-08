import React from 'react';
import Header from './Header';
import MenuBar from './MenuBar';
import Footer from './Footer';
import useAuth from "../../hooks/useAuth";

const Layout = ({ children }) => {
  const {id, username, role} = useAuth()

  if (role === 'Manager') {
      return (
        <div className="flex flex-col min-h-screen">
          <Header />
          <MenuBar />
          <main className="flex-grow container mx-auto px-4 py-8" style={{paddingTop: '5px', paddingBottom: '130px'}}>
            {children}
          </main>
          <Footer />
        </div>
      );
  } else {
      return (
        <div className="flex flex-col min-h-screen">
          <Header />
          <MenuBar />
          <main className="flex-grow container mx-auto px-4 py-8" style={{paddingTop: '5px', paddingBottom: '130px'}}>
            {children}
          </main>
          <Footer />
        </div>
      );
    };
  }

export default Layout;