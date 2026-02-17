-- ============================================================================
-- AS400 Pizza Enterprise - Database Setup Script
-- ============================================================================
-- Description: Complete DDL script for creating the Pizza Enterprise database
--              on IBM AS400/iSeries system
-- Library: PIZZALIB
-- Database: DB2 for i (AS400)
-- ============================================================================

-- ============================================================================
-- STEP 1: CREATE LIBRARY (SCHEMA)
-- ============================================================================

-- Create library to hold all tables
CREATE SCHEMA PIZZALIB;

COMMENT ON SCHEMA PIZZALIB IS 'Pizza Enterprise System - Main Library';

-- ============================================================================
-- STEP 2: CREATE TABLES
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Table: CLIENTES (Customers)
-- Description: Stores customer information including contact details and address
-- ----------------------------------------------------------------------------
CREATE TABLE PIZZALIB.CLIENTES (
    ID                  CHAR(36)        NOT NULL,
    FIRST_NAME          VARCHAR(100)    NOT NULL,
    LAST_NAME           VARCHAR(100)    NOT NULL,
    EMAIL               VARCHAR(200)    NOT NULL,
    PHONE_NUMBER        VARCHAR(20)     NOT NULL,
    DEFAULT_STREET      VARCHAR(200),
    DEFAULT_CITY        VARCHAR(100),
    DEFAULT_STATE       VARCHAR(50),
    DEFAULT_ZIPCODE     VARCHAR(20),
    DEFAULT_COUNTRY     VARCHAR(50),
    IS_ACTIVE           INT             NOT NULL DEFAULT 1,
    CREATED_AT          TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UPDATED_AT          TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT PK_CLIENTES PRIMARY KEY (ID)
);

LABEL ON TABLE PIZZALIB.CLIENTES IS 'Customers Table';
LABEL ON COLUMN PIZZALIB.CLIENTES.ID IS 'Unique Customer ID (GUID)';
LABEL ON COLUMN PIZZALIB.CLIENTES.FIRST_NAME IS 'Customer First Name';
LABEL ON COLUMN PIZZALIB.CLIENTES.LAST_NAME IS 'Customer Last Name';
LABEL ON COLUMN PIZZALIB.CLIENTES.EMAIL IS 'Customer Email Address';
LABEL ON COLUMN PIZZALIB.CLIENTES.PHONE_NUMBER IS 'Customer Phone Number';
LABEL ON COLUMN PIZZALIB.CLIENTES.IS_ACTIVE IS 'Active Status (1=Active, 0=Inactive)';

-- ----------------------------------------------------------------------------
-- Table: PIZZAS
-- Description: Stores pizza menu items with pricing and availability
-- ----------------------------------------------------------------------------
CREATE TABLE PIZZALIB.PIZZAS (
    ID              CHAR(36)        NOT NULL,
    NAME            VARCHAR(200)    NOT NULL,
    DESCRIPTION     VARCHAR(500),
    BASE_PRICE      DECIMAL(18,2)   NOT NULL,
    CURRENCY        VARCHAR(3)      NOT NULL DEFAULT 'USD',
    SIZE            INT             NOT NULL,
    IS_AVAILABLE    INT             NOT NULL DEFAULT 1,
    CREATED_AT      TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UPDATED_AT      TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT PK_PIZZAS PRIMARY KEY (ID)
);

LABEL ON TABLE PIZZALIB.PIZZAS IS 'Pizza Menu Items';
LABEL ON COLUMN PIZZALIB.PIZZAS.ID IS 'Unique Pizza ID (GUID)';
LABEL ON COLUMN PIZZALIB.PIZZAS.NAME IS 'Pizza Name';
LABEL ON COLUMN PIZZALIB.PIZZAS.DESCRIPTION IS 'Pizza Description';
LABEL ON COLUMN PIZZALIB.PIZZAS.BASE_PRICE IS 'Base Price Amount';
LABEL ON COLUMN PIZZALIB.PIZZAS.CURRENCY IS 'Currency Code (ISO 4217)';
LABEL ON COLUMN PIZZALIB.PIZZAS.SIZE IS 'Pizza Size (0=Small,1=Medium,2=Large,3=XL)';
LABEL ON COLUMN PIZZALIB.PIZZAS.IS_AVAILABLE IS 'Available for Order (1=Yes, 0=No)';

-- ----------------------------------------------------------------------------
-- Table: REPARTIDORES (Delivery Persons)
-- Description: Stores delivery personnel information
-- ----------------------------------------------------------------------------
CREATE TABLE PIZZALIB.REPARTIDORES (
    ID                  CHAR(36)        NOT NULL,
    FIRST_NAME          VARCHAR(100)    NOT NULL,
    LAST_NAME           VARCHAR(100)    NOT NULL,
    PHONE_NUMBER        VARCHAR(20)     NOT NULL,
    VEHICLE_PLATE       VARCHAR(20)     NOT NULL,
    IS_AVAILABLE        INT             NOT NULL DEFAULT 1,
    IS_ACTIVE           INT             NOT NULL DEFAULT 1,
    CREATED_AT          TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UPDATED_AT          TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT PK_REPARTIDORES PRIMARY KEY (ID)
);

LABEL ON TABLE PIZZALIB.REPARTIDORES IS 'Delivery Personnel';
LABEL ON COLUMN PIZZALIB.REPARTIDORES.ID IS 'Unique Delivery Person ID (GUID)';
LABEL ON COLUMN PIZZALIB.REPARTIDORES.FIRST_NAME IS 'First Name';
LABEL ON COLUMN PIZZALIB.REPARTIDORES.LAST_NAME IS 'Last Name';
LABEL ON COLUMN PIZZALIB.REPARTIDORES.PHONE_NUMBER IS 'Phone Number';
LABEL ON COLUMN PIZZALIB.REPARTIDORES.VEHICLE_PLATE IS 'Vehicle License Plate';
LABEL ON COLUMN PIZZALIB.REPARTIDORES.IS_AVAILABLE IS 'Currently Available (1=Yes, 0=No)';
LABEL ON COLUMN PIZZALIB.REPARTIDORES.IS_ACTIVE IS 'Active Employee (1=Yes, 0=No)';

-- ----------------------------------------------------------------------------
-- Table: PEDIDOS (Orders)
-- Description: Stores customer orders with delivery information
-- ----------------------------------------------------------------------------
CREATE TABLE PIZZALIB.PEDIDOS (
    ID                      CHAR(36)        NOT NULL,
    ORDER_NUMBER            VARCHAR(50)     NOT NULL,
    CUSTOMER_ID             CHAR(36)        NOT NULL,
    ORDER_DATE              TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    STATUS                  INT             NOT NULL DEFAULT 0,
    TOTAL_AMOUNT            DECIMAL(18,2)   NOT NULL,
    CURRENCY                VARCHAR(3)      NOT NULL DEFAULT 'USD',
    DELIVERY_STREET         VARCHAR(200)    NOT NULL,
    DELIVERY_CITY           VARCHAR(100)    NOT NULL,
    DELIVERY_STATE          VARCHAR(50)     NOT NULL,
    DELIVERY_ZIPCODE        VARCHAR(20)     NOT NULL,
    DELIVERY_COUNTRY        VARCHAR(50)     NOT NULL,
    DELIVERY_PERSON_ID      CHAR(36),
    CREATED_AT              TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UPDATED_AT              TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT PK_PEDIDOS PRIMARY KEY (ID),
    CONSTRAINT FK_PEDIDOS_CUSTOMER FOREIGN KEY (CUSTOMER_ID) 
        REFERENCES PIZZALIB.CLIENTES(ID),
    CONSTRAINT UK_PEDIDOS_ORDERNUMBER UNIQUE (ORDER_NUMBER)
);

LABEL ON TABLE PIZZALIB.PEDIDOS IS 'Customer Orders';
LABEL ON COLUMN PIZZALIB.PEDIDOS.ID IS 'Unique Order ID (GUID)';
LABEL ON COLUMN PIZZALIB.PEDIDOS.ORDER_NUMBER IS 'Human-Readable Order Number';
LABEL ON COLUMN PIZZALIB.PEDIDOS.CUSTOMER_ID IS 'Customer ID (FK)';
LABEL ON COLUMN PIZZALIB.PEDIDOS.ORDER_DATE IS 'Order Placement Date/Time';
LABEL ON COLUMN PIZZALIB.PEDIDOS.STATUS IS 'Order Status (0=Pending,1=Confirmed,2=InPrep,3=Ready,4=InDelivery,5=Delivered,6=Cancelled)';
LABEL ON COLUMN PIZZALIB.PEDIDOS.TOTAL_AMOUNT IS 'Total Order Amount';
LABEL ON COLUMN PIZZALIB.PEDIDOS.CURRENCY IS 'Currency Code';
LABEL ON COLUMN PIZZALIB.PEDIDOS.DELIVERY_PERSON_ID IS 'Assigned Delivery Person (FK)';

-- ----------------------------------------------------------------------------
-- Table: PEDIDOS_DET (Order Items/Details)
-- Description: Stores individual pizza items within orders
-- ----------------------------------------------------------------------------
CREATE TABLE PIZZALIB.PEDIDOS_DET (
    ID              CHAR(36)        NOT NULL,
    ORDER_ID        CHAR(36)        NOT NULL,
    PIZZA_ID        CHAR(36)        NOT NULL,
    PIZZA_NAME      VARCHAR(200)    NOT NULL,
    QUANTITY        INT             NOT NULL,
    UNIT_PRICE      DECIMAL(18,2)   NOT NULL,
    CURRENCY        VARCHAR(3)      NOT NULL DEFAULT 'USD',
    SUBTOTAL        DECIMAL(18,2)   NOT NULL,
    CREATED_AT      TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UPDATED_AT      TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT PK_PEDIDOS_DET PRIMARY KEY (ID),
    CONSTRAINT FK_PEDIDOS_DET_ORDER FOREIGN KEY (ORDER_ID) 
        REFERENCES PIZZALIB.PEDIDOS(ID)
);

LABEL ON TABLE PIZZALIB.PEDIDOS_DET IS 'Order Line Items';
LABEL ON COLUMN PIZZALIB.PEDIDOS_DET.ID IS 'Unique Order Item ID (GUID)';
LABEL ON COLUMN PIZZALIB.PEDIDOS_DET.ORDER_ID IS 'Order ID (FK)';
LABEL ON COLUMN PIZZALIB.PEDIDOS_DET.PIZZA_ID IS 'Pizza ID Reference';
LABEL ON COLUMN PIZZALIB.PEDIDOS_DET.PIZZA_NAME IS 'Pizza Name (Snapshot)';
LABEL ON COLUMN PIZZALIB.PEDIDOS_DET.QUANTITY IS 'Quantity Ordered';
LABEL ON COLUMN PIZZALIB.PEDIDOS_DET.UNIT_PRICE IS 'Unit Price Amount';
LABEL ON COLUMN PIZZALIB.PEDIDOS_DET.SUBTOTAL IS 'Line Item Subtotal';

-- ----------------------------------------------------------------------------
-- Table: PAGOS (Payments)
-- Description: Stores payment transactions for orders
-- ----------------------------------------------------------------------------
CREATE TABLE PIZZALIB.PAGOS (
    ID                  CHAR(36)        NOT NULL,
    ORDER_ID            CHAR(36)        NOT NULL,
    AMOUNT              DECIMAL(18,2)   NOT NULL,
    CURRENCY            VARCHAR(3)      NOT NULL DEFAULT 'USD',
    METHOD              INT             NOT NULL,
    STATUS              INT             NOT NULL DEFAULT 0,
    TRANSACTION_ID      VARCHAR(100),
    COMPLETED_AT        TIMESTAMP,
    CREATED_AT          TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UPDATED_AT          TIMESTAMP       NOT NULL DEFAULT CURRENT_TIMESTAMP,
    
    CONSTRAINT PK_PAGOS PRIMARY KEY (ID),
    CONSTRAINT FK_PAGOS_ORDER FOREIGN KEY (ORDER_ID) 
        REFERENCES PIZZALIB.PEDIDOS(ID)
);

LABEL ON TABLE PIZZALIB.PAGOS IS 'Payment Transactions';
LABEL ON COLUMN PIZZALIB.PAGOS.ID IS 'Unique Payment ID (GUID)';
LABEL ON COLUMN PIZZALIB.PAGOS.ORDER_ID IS 'Order ID (FK)';
LABEL ON COLUMN PIZZALIB.PAGOS.AMOUNT IS 'Payment Amount';
LABEL ON COLUMN PIZZALIB.PAGOS.CURRENCY IS 'Currency Code';
LABEL ON COLUMN PIZZALIB.PAGOS.METHOD IS 'Payment Method (0=Cash,1=Credit,2=Debit,3=Online)';
LABEL ON COLUMN PIZZALIB.PAGOS.STATUS IS 'Payment Status (0=Pending,1=Completed,2=Failed,3=Refunded)';
LABEL ON COLUMN PIZZALIB.PAGOS.TRANSACTION_ID IS 'External Transaction ID';
LABEL ON COLUMN PIZZALIB.PAGOS.COMPLETED_AT IS 'Payment Completion Date/Time';

-- ============================================================================
-- STEP 3: CREATE INDEXES
-- ============================================================================

-- Customer indexes
CREATE INDEX PIZZALIB.IDX_CLIENTES_EMAIL 
    ON PIZZALIB.CLIENTES(EMAIL);

CREATE INDEX PIZZALIB.IDX_CLIENTES_ACTIVE 
    ON PIZZALIB.CLIENTES(IS_ACTIVE);

-- Pizza indexes
CREATE INDEX PIZZALIB.IDX_PIZZAS_AVAILABLE 
    ON PIZZALIB.PIZZAS(IS_AVAILABLE);

CREATE INDEX PIZZALIB.IDX_PIZZAS_SIZE 
    ON PIZZALIB.PIZZAS(SIZE);

-- Order indexes
CREATE INDEX PIZZALIB.IDX_PEDIDOS_CUSTOMER 
    ON PIZZALIB.PEDIDOS(CUSTOMER_ID);

CREATE INDEX PIZZALIB.IDX_PEDIDOS_STATUS 
    ON PIZZALIB.PEDIDOS(STATUS);

CREATE INDEX PIZZALIB.IDX_PEDIDOS_ORDERDATE 
    ON PIZZALIB.PEDIDOS(ORDER_DATE);

CREATE INDEX PIZZALIB.IDX_PEDIDOS_DELIVERYPERSON 
    ON PIZZALIB.PEDIDOS(DELIVERY_PERSON_ID);

-- Order items indexes
CREATE INDEX PIZZALIB.IDX_PEDIDOS_DET_ORDER 
    ON PIZZALIB.PEDIDOS_DET(ORDER_ID);

CREATE INDEX PIZZALIB.IDX_PEDIDOS_DET_PIZZA 
    ON PIZZALIB.PEDIDOS_DET(PIZZA_ID);

-- Payment indexes
CREATE INDEX PIZZALIB.IDX_PAGOS_ORDER 
    ON PIZZALIB.PAGOS(ORDER_ID);

CREATE INDEX PIZZALIB.IDX_PAGOS_STATUS 
    ON PIZZALIB.PAGOS(STATUS);

CREATE INDEX PIZZALIB.IDX_PAGOS_METHOD 
    ON PIZZALIB.PAGOS(METHOD);

-- Delivery person indexes
CREATE INDEX PIZZALIB.IDX_REPARTIDORES_AVAILABLE 
    ON PIZZALIB.REPARTIDORES(IS_AVAILABLE);

CREATE INDEX PIZZALIB.IDX_REPARTIDORES_ACTIVE 
    ON PIZZALIB.REPARTIDORES(IS_ACTIVE);

-- ============================================================================
-- STEP 4: INSERT SAMPLE DATA
-- ============================================================================

-- ----------------------------------------------------------------------------
-- Sample Customers
-- ----------------------------------------------------------------------------
INSERT INTO PIZZALIB.CLIENTES 
    (ID, FIRST_NAME, LAST_NAME, EMAIL, PHONE_NUMBER, DEFAULT_STREET, DEFAULT_CITY, DEFAULT_STATE, DEFAULT_ZIPCODE, DEFAULT_COUNTRY, IS_ACTIVE, CREATED_AT, UPDATED_AT)
VALUES
    ('11111111-1111-1111-1111-111111111111', 'John', 'Doe', 'john.doe@example.com', '+1-555-0101', 
     '123 Main Street', 'Springfield', 'Illinois', '62701', 'USA', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('22222222-2222-2222-2222-222222222222', 'Jane', 'Smith', 'jane.smith@example.com', '+1-555-0102', 
     '456 Oak Avenue', 'Chicago', 'Illinois', '60601', 'USA', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('33333333-3333-3333-3333-333333333333', 'Bob', 'Johnson', 'bob.johnson@example.com', '+1-555-0103', 
     '789 Elm Street', 'Peoria', 'Illinois', '61602', 'USA', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('44444444-4444-4444-4444-444444444444', 'Alice', 'Williams', 'alice.williams@example.com', '+1-555-0104', 
     '321 Pine Road', 'Rockford', 'Illinois', '61101', 'USA', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('55555555-5555-5555-5555-555555555555', 'Charlie', 'Brown', 'charlie.brown@example.com', '+1-555-0105', 
     '654 Maple Drive', 'Naperville', 'Illinois', '60540', 'USA', 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- ----------------------------------------------------------------------------
-- Sample Pizzas
-- ----------------------------------------------------------------------------
INSERT INTO PIZZALIB.PIZZAS 
    (ID, NAME, DESCRIPTION, BASE_PRICE, CURRENCY, SIZE, IS_AVAILABLE, CREATED_AT, UPDATED_AT)
VALUES
    ('aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa', 'Margherita', 
     'Classic pizza with fresh tomatoes, mozzarella cheese, and basil', 
     12.99, 'USD', 2, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb', 'Pepperoni', 
     'Traditional pepperoni with mozzarella cheese and tomato sauce', 
     14.99, 'USD', 2, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('cccccccc-cccc-cccc-cccc-cccccccccccc', 'Hawaiian', 
     'Ham and pineapple with mozzarella cheese', 
     15.99, 'USD', 2, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('dddddddd-dddd-dddd-dddd-dddddddddddd', 'Vegetarian', 
     'Bell peppers, onions, mushrooms, olives, and tomatoes', 
     13.99, 'USD', 2, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee', 'Meat Lovers', 
     'Pepperoni, sausage, bacon, and ham with extra cheese', 
     17.99, 'USD', 2, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('ffffffff-ffff-ffff-ffff-ffffffffffff', 'BBQ Chicken', 
     'Grilled chicken, BBQ sauce, red onions, and cilantro', 
     16.99, 'USD', 2, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('gggggggg-gggg-gggg-gggg-gggggggggggg', 'Supreme', 
     'Pepperoni, sausage, peppers, onions, mushrooms, and olives', 
     18.99, 'USD', 3, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
     
    ('hhhhhhhh-hhhh-hhhh-hhhh-hhhhhhhhhhhh', 'Four Cheese', 
     'Mozzarella, parmesan, gorgonzola, and fontina cheese blend', 
     15.99, 'USD', 2, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- ----------------------------------------------------------------------------
-- Sample Delivery Persons
-- ----------------------------------------------------------------------------
INSERT INTO PIZZALIB.REPARTIDORES 
    (ID, FIRST_NAME, LAST_NAME, PHONE_NUMBER, VEHICLE_PLATE, IS_AVAILABLE, IS_ACTIVE, CREATED_AT, UPDATED_AT)
VALUES
    ('d1111111-1111-1111-1111-111111111111', 'Mike', 'Rodriguez', '+1-555-0201', 'ABC-1234', 1, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
    ('d2222222-2222-2222-2222-222222222222', 'Sarah', 'Martinez', '+1-555-0202', 'XYZ-5678', 1, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
    ('d3333333-3333-3333-3333-333333333333', 'David', 'Garcia', '+1-555-0203', 'DEF-9012', 1, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
    ('d4444444-4444-4444-4444-444444444444', 'Emily', 'Lopez', '+1-555-0204', 'GHI-3456', 0, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP),
    ('d5555555-5555-5555-5555-555555555555', 'James', 'Wilson', '+1-555-0205', 'JKL-7890', 1, 1, CURRENT_TIMESTAMP, CURRENT_TIMESTAMP);

-- ============================================================================
-- STEP 5: VERIFICATION QUERIES
-- ============================================================================

-- Verify table creation
SELECT TABLE_NAME, TABLE_TEXT 
FROM QSYS2.SYSTABLES 
WHERE TABLE_SCHEMA = 'PIZZALIB'
ORDER BY TABLE_NAME;

-- Verify sample data counts
SELECT 
    (SELECT COUNT(*) FROM PIZZALIB.CLIENTES) AS CUSTOMER_COUNT,
    (SELECT COUNT(*) FROM PIZZALIB.PIZZAS) AS PIZZA_COUNT,
    (SELECT COUNT(*) FROM PIZZALIB.REPARTIDORES) AS DELIVERY_PERSON_COUNT,
    (SELECT COUNT(*) FROM PIZZALIB.PEDIDOS) AS ORDER_COUNT,
    (SELECT COUNT(*) FROM PIZZALIB.PEDIDOS_DET) AS ORDER_ITEM_COUNT,
    (SELECT COUNT(*) FROM PIZZALIB.PAGOS) AS PAYMENT_COUNT
FROM SYSIBM.SYSDUMMY1;

-- Verify indexes
SELECT INDEX_NAME, TABLE_NAME, COLUMN_NAME
FROM QSYS2.SYSINDEXES 
WHERE TABLE_SCHEMA = 'PIZZALIB'
ORDER BY TABLE_NAME, INDEX_NAME;

-- ============================================================================
-- NOTES
-- ============================================================================
-- 
-- 1. GUID Storage: All IDs are stored as CHAR(36) to accommodate GUIDs from .NET
-- 2. Boolean Fields: Stored as INT with 1 (true) / 0 (false) values
-- 3. Enums: Stored as INT values (see documentation for mappings)
-- 4. Timestamps: All tables include CREATED_AT and UPDATED_AT for auditing
-- 5. Currency: ISO 4217 codes (USD, EUR, etc.) stored as VARCHAR(3)
-- 6. Decimal Precision: DECIMAL(18,2) used for all monetary amounts
-- 7. Column Names: Use snake_case with underscores (e.g., ORDER_NUMBER, CUSTOMER_ID)
--    to match AS400TableConstants in .NET application
-- 
-- ENUM MAPPINGS:
-- ---------------
-- OrderStatus:     0=Pending, 1=Confirmed, 2=InPreparation, 3=ReadyForDelivery, 
--                  4=InDelivery, 5=Delivered, 6=Cancelled
-- 
-- PaymentMethod:   0=Cash, 1=CreditCard, 2=DebitCard, 3=Online
-- 
-- PaymentStatus:   0=Pending, 1=Completed, 2=Failed, 3=Refunded
-- 
-- PizzaSize:       0=Small, 1=Medium, 2=Large, 3=ExtraLarge
-- 
-- FOREIGN KEY RELATIONSHIPS:
-- --------------------------
-- PEDIDOS.CUSTOMER_ID -> CLIENTES.ID
-- PEDIDOS_DET.ORDER_ID -> PEDIDOS.ID
-- PAGOS.ORDER_ID -> PEDIDOS.ID
-- 
-- Note: DELIVERY_PERSON_ID and PIZZA_ID are logical foreign keys but not enforced
--       with constraints to allow flexibility in the application layer.
-- 
-- ============================================================================
-- END OF SCRIPT
-- ============================================================================
