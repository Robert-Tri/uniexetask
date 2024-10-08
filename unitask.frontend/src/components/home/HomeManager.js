import React from 'react';

const HomeManager = () => {
  const sampleData = [
    { id: 1, name: 'Project A', status: 'Active' },
    { id: 2, name: 'Project B', status: 'Completed' },
    { id: 3, name: 'Project C', status: 'In Progress' },
  ];

  return (
    <div className="mt-4 ml-64"> {/* Giữ khoảng cách cho sidebar */}
      <h2 className="text-2xl font-bold mb-4">Project List</h2>
      <table className="min-w-full bg-white border border-gray-300">
        <thead>
          <tr>
            <th className="py-2 px-4 border-b text-left">ID</th>
            <th className="py-2 px-4 border-b text-left">Name</th>
            <th className="py-2 px-4 border-b text-left">Status</th>
          </tr>
        </thead>
        <tbody>
          {sampleData.map((project) => (
            <tr key={project.id}>
              <td className="py-2 px-4 border-b">{project.id}</td>
              <td className="py-2 px-4 border-b">{project.name}</td>
              <td className="py-2 px-4 border-b">{project.status}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default HomeManager;
