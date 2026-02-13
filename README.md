# ProxyCare - Canada

## Objective

ProxyCare-Canada is a web-based healthcare information platform designed to improve patient decision-making by providing visibility into hospital queue lengths and estimated waiting times. The motivation behind this project stems from the increasing congestion in healthcare facilities and the lack of transparency regarding patient wait times, which often results in delayed treatment and inefficient patient flow.

The system enables patients to search for nearby hospitals through a public web interface and view near real-time queue information before visiting a facility. In parallel, a secure administrative interface allows authorized hospital representatives to update queue status, while a master administrator manages hospital onboarding and approval of hospital representatives. 

The platform follows a layered architecture with strict separation between public access and administrative functions, ensuring security, scalability, and maintainability.

## Project Motivation
The Canadian healthcare system is currently facing significant challenges, including:

* Overcrowded emergency rooms
* Excessive patient waiting times
* Lack of transparency regarding hospital queue status
* Patients travelling to healthcare facilities without knowing expected delays

At present, patients have no centralized platform to view hospital wait times in advance. ProxyCare-Canada is motivated by the need to provide data-driven visibility that enables patients to choose facilities with lower congestion and faster access to care.

> Recent reports indicate approximately 23,746–24,000 deaths occurred while patients were waiting for care between April 2024 and March 2025, emphasizing the urgency of improving patient flow and decision-making.

## Solution
This project successfully delivered a comprehensive, user-centric hospital discovery and monitoring system by integrating modern web technologies, geospatial services, and real-time data mechanisms. The key achievements of the project are summarized below:

1. **End-to-End Hospital Discovery Platform**
    * Designed and implemented a patient-facing hospital search system enabling users to locate nearby hospitals based on address or current location.
    * Integrated map-based visualization using OpenStreetMap and Leaflet, allowing intuitive geographic exploration of healthcare facilities.

2. **Real-Time Operational Insights**
    * Successfully incorporated queue length and estimated wait-time metrics, providing users with actionable information to make informed healthcare decisions.
    * Enabled near real-time data updates through event-driven mechanisms, ensuring timely propagation of changes made by hospital representatives.

3. **Role-Based Data Management**
    * Implemented a hospital representative (HospitalRep) role with approval-based access control to securely update hospital operational data.
    * Ensured data integrity and authorization through identity-linked representatives and administrative approval workflows.

4. **Scalable and Performant Architecture**
    * Adopted a modular Blazor-based frontend and RESTful backend architecture, facilitating maintainability and future expansion.
    * Optimized hospital data loading using batched pagination and radius-based filtering, improving performance when handling large datasets.

5. **Enhanced User Experience and Accessibility**
    * Designed responsive, mobile-friendly UI components including expandable hospital cards, search suggestions, and interactive filters.

6. **Cloud-Ready Deployment**
    * Prepared the application for deployment on Microsoft Azure App Service, with Azure SQL Database as the backend datastore.
    * Followed best practices for configuration, scalability, and environment separation suitable for production deployment.

