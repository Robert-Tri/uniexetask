import React from 'react';

const Footer = () => {
  return (
    <footer className="bg-gray-800 text-white fixed bottom-0 left-0 w-full" style={{ padding: '10px 0', maxHeight: '125px', zIndex: 50 }}>
      <div className="container mx-auto px-16">
        <div className="flex flex-wrap justify-between">
          <div className="w-full md:w-1/2 text-center md:text-left">
            <h5 className="uppercase mb-1 font-bold text-center">Links</h5>
            <ul className="mb-2 text-center">
              <li className="mt-1">
                <a href="https://flm.fpt.edu.vn/" className="hover:underline text-gray-300 hover:text-white">FPT Materials</a>
              </li>
              <li className="mt-1">
                <a href="https://fu-edunext.fpt.edu.vn/" className="hover:underline text-gray-300 hover:text-white">FU Edunext</a>
              </li>
            </ul>
          </div>
          <div className="w-full md:w-1/2 text-center md:text-left">
            <h5 className="uppercase mb-1 font-bold text-center">Contact</h5>
            <ul className="mb-2 text-center">
              <li className="mt-1">
                <a href="#" className="hover:underline text-gray-300 hover:text-white">UniEXETask@gmail.com</a>
              </li>
              <li className="mt-1">
                <a href="#" className="hover:underline text-gray-300 hover:text-white">Tel: +84 867892130</a>
              </li>
            </ul>
          </div>
        </div>
      </div>
      <div className="container mx-auto px-16">
        <div className="border-t-2 border-gray-300 flex flex-col items-end">
          <div className="sm:w-2/3 text-center py-1" style={{ paddingLeft: '140px' }}>
            <p className="text-sm text-gray-300 font-bold mb-0 text-left">
              © 2024 by UniEXETask FPTU
            </p>
          </div>
        </div>
      </div>
    </footer>
  );
}

export default Footer;
