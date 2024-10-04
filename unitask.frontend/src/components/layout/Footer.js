import React from 'react';
import { Facebook, Twitter, Instagram } from 'lucide-react';

const Footer = () => {
  return (
    <footer className="bg-gray-800 text-white">
      <div className="container mx-auto px-6 py-8">
        <div className="flex flex-wrap">
          <div className="w-full md:w-1/4 text-center md:text-left">
            <h5 className="uppercase mb-6 font-bold">Links</h5>
            <ul className="mb-4">
              <li className="mt-2">
                <a href="#" className="hover:underline text-gray-300 hover:text-white">FAQ</a>
              </li>
              <li className="mt-2">
                <a href="#" className="hover:underline text-gray-300 hover:text-white">Help</a>
              </li>
              <li className="mt-2">
                <a href="#" className="hover:underline text-gray-300 hover:text-white">Support</a>
              </li>
            </ul>
          </div>
          <div className="w-full md:w-1/4 text-center md:text-left">
            <h5 className="uppercase mb-6 font-bold">Legal</h5>
            <ul className="mb-4">
              <li className="mt-2">
                <a href="#" className="hover:underline text-gray-300 hover:text-white">Terms</a>
              </li>
              <li className="mt-2">
                <a href="#" className="hover:underline text-gray-300 hover:text-white">Privacy</a>
              </li>
            </ul>
          </div>
          <div className="w-full md:w-1/4 text-center md:text-left">
            <h5 className="uppercase mb-6 font-bold">Company</h5>
            <ul className="mb-4">
              <li className="mt-2">
                <a href="#" className="hover:underline text-gray-300 hover:text-white">About Us</a>
              </li>
              <li className="mt-2">
                <a href="#" className="hover:underline text-gray-300 hover:text-white">Contact</a>
              </li>
            </ul>
          </div>
          <div className="w-full md:w-1/4 text-center md:text-left">
            <h5 className="uppercase mb-6 font-bold">Social</h5>
            <ul className="mb-4 flex justify-center md:justify-start">
              <li className="mr-4">
                <a href="#" className="text-gray-300 hover:text-white">
                  <Facebook size={20} />
                </a>
              </li>
              <li className="mr-4">
                <a href="#" className="text-gray-300 hover:text-white">
                  <Twitter size={20} />
                </a>
              </li>
              <li className="mr-4">
                <a href="#" className="text-gray-300 hover:text-white">
                  <Instagram size={20} />
                </a>
              </li>
            </ul>
          </div>
        </div>
      </div>
      <div className="container mx-auto px-6">
        <div className="mt-16 border-t-2 border-gray-300 flex flex-col items-center">
          <div className="sm:w-2/3 text-center py-6">
            <p className="text-sm text-gray-300 font-bold mb-2">
              © 2024 by Project Management System
            </p>
          </div>
        </div>
      </div>
    </footer>
  );
}

export default Footer;