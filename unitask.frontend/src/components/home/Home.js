import React from 'react';
import { motion } from 'framer-motion';
import { Users, Briefcase, Calendar, FileText, MessageSquare, HelpCircle, Lightbulb } from 'lucide-react';

const Home = () => {
  const features = [
    { icon: <Users size={32} />, title: 'Group Management', description: 'Form and manage entrepreneurship groups', color: 'bg-gradient-to-br from-pink-400 to-red-500' },
    { icon: <Briefcase size={32} />, title: 'Project Management', description: 'Create and track entrepreneurial projects', color: 'bg-gradient-to-br from-green-400 to-blue-500' },
    { icon: <Calendar size={32} />, title: 'Task Management', description: 'Assign and monitor project tasks', color: 'bg-gradient-to-br from-purple-400 to-indigo-500' },
    { icon: <FileText size={32} />, title: 'Resource Management', description: 'Upload and share project documents', color: 'bg-gradient-to-br from-yellow-400 to-orange-500' },
    { icon: <MessageSquare size={32} />, title: 'Communication', description: 'Chat and collaborate with team members', color: 'bg-gradient-to-br from-teal-400 to-cyan-500' },
  ];

  const containerVariants = {
    hidden: { opacity: 0 },
    visible: { 
      opacity: 1,
      transition: { 
        staggerChildren: 0.1,
        delayChildren: 0.3
      }
    }
  };

  const itemVariants = {
    hidden: { y: 20, opacity: 0 },
    visible: { 
      y: 0, 
      opacity: 1,
      transition: { type: 'spring', stiffness: 100 }
    }
  };

  return (
    <div className="min-h-screen w-full bg-gradient-to-br from-gray-900 via-purple-900 to-violet-600 text-white">
      <div className="absolute inset-0">
        <div className="absolute left-1/3 top-1/4 w-72 h-72 bg-pink-500 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob"></div>
        <div className="absolute right-1/3 top-1/3 w-72 h-72 bg-yellow-500 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob animation-delay-2000"></div>
        <div className="absolute left-1/2 bottom-1/4 w-72 h-72 bg-blue-500 rounded-full mix-blend-multiply filter blur-xl opacity-70 animate-blob animation-delay-4000"></div>
      </div>
      
      <motion.div 
        className="w-full h-full px-4 py-12 relative z-10"
        initial="hidden"
        animate="visible"
        variants={containerVariants}
      >
        <motion.h1 
          className="text-5xl md:text-6xl font-bold text-center mb-8 text-transparent bg-clip-text bg-gradient-to-r from-pink-500 via-yellow-500 to-cyan-500"
          variants={itemVariants}
        >
          Welcome to UniEXETask
        </motion.h1>
        
        <motion.section className="mb-16 max-w-4xl mx-auto px-4" variants={itemVariants}>
          <h2 className="text-3xl md:text-4xl font-semibold mb-4 text-cyan-300">About UniEXETask</h2>
          <p className="text-lg md:text-xl text-gray-300">
            UniEXETask is your gateway to entrepreneurial success at FPT University. 
            Unleash your innovative ideas and transform them into reality with our comprehensive platform.
          </p>
        </motion.section>

        <motion.section className="mb-16 max-w-6xl mx-auto px-4" variants={containerVariants}>
          <h2 className="text-3xl md:text-4xl font-semibold mb-8 text-yellow-300">Key Features</h2>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            {features.map((feature, index) => (
              <motion.div 
                key={index} 
                className={`${feature.color} p-6 rounded-xl shadow-lg backdrop-blur-md transition-all duration-300`}
                variants={itemVariants}
                whileHover={{ scale: 1.05, rotate: 1 }}
                whileTap={{ scale: 0.95 }}
              >
                <div className="flex items-center mb-4">
                  {feature.icon}
                  <h3 className="text-xl md:text-2xl font-semibold ml-3">{feature.title}</h3>
                </div>
                <p className="text-base md:text-lg">{feature.description}</p>
              </motion.div>
            ))}
          </div>
        </motion.section>

        <motion.section className="mb-16 max-w-4xl mx-auto" variants={itemVariants}>
          <h2 className="text-4xl font-semibold mb-4 text-green-300">Getting Started</h2>
          <motion.div 
            className="bg-gradient-to-br from-indigo-500 to-purple-600 p-8 rounded-xl shadow-lg backdrop-blur-md"
            whileHover={{ scale: 1.02 }}
          >
            <h3 className="text-2xl font-semibold mb-4 flex items-center">
              <Lightbulb className="mr-2" /> Quick Guide
            </h3>
            <ul className="list-disc list-inside space-y-3 text-lg">
              <motion.li variants={itemVariants}>Explore the intuitive navigation menu</motion.li>
              <motion.li variants={itemVariants}>Join or create a dynamic entrepreneurship group</motion.li>
              <motion.li variants={itemVariants}>Launch your project and define your groundbreaking idea</motion.li>
              <motion.li variants={itemVariants}>Delegate tasks and monitor real-time progress</motion.li>
              <motion.li variants={itemVariants}>Share resources to fuel your project's growth</motion.li>
              <motion.li variants={itemVariants}>Collaborate seamlessly through our integrated chat system</motion.li>
            </ul>
          </motion.div>
        </motion.section>

        <motion.section className="max-w-4xl mx-auto" variants={itemVariants}>
          <h2 className="text-4xl font-semibold mb-4 text-pink-300">Need Help?</h2>
          <motion.div 
            className="bg-gradient-to-br from-red-500 to-pink-600 p-6 rounded-xl shadow-lg backdrop-blur-md flex items-center"
            whileHover={{ scale: 1.02 }}
          >
            <HelpCircle size={48} className="mr-4" />
            <p className="text-xl">
              Our support team and mentors are here to guide you every step of the way. 
              Don't hesitate to reach out and unlock your full entrepreneurial potential!
            </p>
          </motion.div>
        </motion.section>
      </motion.div>
    </div>
  );
};

export default Home;