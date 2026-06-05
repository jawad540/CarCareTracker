# 🚗 دليل التثبيت الكامل — Car Care Tracker (نسخة SQL Server)

نظام إدارة مصاريف وصيانة المركبات
ASP.NET MVC 5 + SQL Server + JWT
جامعة عمان العربية / كلية تكنولوجيا المعلومات

> ✅ هاي النسخة تستخدم **SQL Server** بدل Oracle — أسهل بكثير في التثبيت!

---

# 📋 البرامج المطلوبة

| # | البرنامج | الحجم | الوقت |
|---|----------|-------|-------|
| 1 | **SQL Server 2022 Express** (قاعدة البيانات) | ~250 MB | 10 دقايق |
| 2 | **SQL Server Management Studio (SSMS)** (أداة الإدارة) | ~700 MB | 10 دقايق |
| 3 | **Visual Studio 2022 Community** (بيئة التطوير) | ~3-5 GB | 30-60 دقيقة |

> 💡 **ملاحظة مهمة:** لو عندك Visual Studio منصّب، غالباً **SQL Server LocalDB موجود تلقائياً** معه! يعني ممكن تتخطّى تثبيت SQL Server كلياً (شوف الخيار السريع تحت).

---

# ⚡ الخيار السريع (لو عندك Visual Studio)

Visual Studio بيجي معه نسخة مصغّرة من SQL Server اسمها **LocalDB**. لو مثبّت VS، جرّب هاد أول:

1. افتح **Web.config** في المشروع.
2. غيّر سطر الاتصال لهذا:
   ```xml
   <add name="DefaultConnection"
        connectionString="Server=(localdb)\MSSQLLocalDB;Database=CarCareTracker;Integrated Security=True;TrustServerCertificate=True;"
        providerName="System.Data.SqlClient" />
   ```
3. روح مباشرة للجزء الثالث (تشغيل سكربت قاعدة البيانات) — بتقدر تشغّله من داخل Visual Studio عبر **SQL Server Object Explorer**.

لو ما زبط معك LocalDB، كمّل التثبيت العادي تحت 👇

---

# 🟦 الجزء الأول: تثبيت SQL Server Express

## الخطوة 1.1 — التحميل

1. روح على: **https://www.microsoft.com/en-us/sql-server/sql-server-downloads**
2. تحت **Express** (مجاني) → اضغط **Download now**.
3. رح ينزل ملف صغير (مُثبّت أولي).

## الخطوة 1.2 — التثبيت

1. شغّل الملف اللي نزّلته.
2. رح يطلع 3 خيارات تثبيت: اختر **Basic** (الأسهل).
3. وافق على الترخيص → **Accept**.
4. اتركه يحمّل ويثبّت (~250 MB، حوالي 5-10 دقايق).
5. لما يخلّص، رح تطلع شاشة فيها معلومات مهمة:
   - **🔑 Connection String / Instance Name:** عادةً بيكون `localhost\SQLEXPRESS` أو `.\SQLEXPRESS`
   - **خُد screenshot للشاشة!**
6. اضغط **Close**.

> 📝 **احفظ اسم الـ Instance** — رح نحتاجه. غالباً بيكون `SQLEXPRESS`.

---

# 🟩 الجزء الثاني: تثبيت SQL Server Management Studio (SSMS)

> هاي الأداة الرسومية لإدارة قاعدة البيانات (متل SQL Developer بس لـ SQL Server).

## الخطوة 2.1 — التحميل

1. روح على: **https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms**
2. اضغط على رابط **Download SSMS**.
3. رح ينزل ملف (~700 MB).

## الخطوة 2.2 — التثبيت

1. شغّل الملف → **Install**.
2. اتركه يخلّص (~10 دقايق) → **Restart** لو طلب.

## الخطوة 2.3 — الاتصال بقاعدة البيانات

1. افتح **SQL Server Management Studio**.
2. رح تطلع نافذة **Connect to Server**:
   - **Server type:** Database Engine
   - **Server name:** `.\SQLEXPRESS` (أو `localhost\SQLEXPRESS`)
   - **Authentication:** Windows Authentication
3. اضغط **Connect**.

✅ **لو اتصل، معناه SQL Server شغّال تمام!**

---

# 🟨 الجزء الثالث: إنشاء قاعدة بيانات المشروع

> أسهل بكثير من Oracle — ما في إنشاء مستخدمين ولا صلاحيات!

## الخطوة 3.1 — تشغيل سكربت الجداول

1. في SSMS (بعد الاتصال)، من القائمة: **File → Open → File**.
2. اختر ملف **`CarCareTracker_Database_SqlServer.sql`**.
3. رح ينفتح محتوى السكربت.
4. اضغط زر **Execute** (أو **F5**).
5. رح تطلع رسائل نجاح، والسكربت:
   - بينشئ قاعدة بيانات اسمها `CarCareTracker`
   - بينشئ 9 جداول + بيانات أولية + 3 Views

## الخطوة 3.2 — التأكد

1. على اليسار في **Object Explorer**، اضغط **Refresh** (أو F5).
2. وسّع **Databases → CarCareTracker → Tables**.
3. لازم تشوف 9 جداول:
   ```
   dbo.FUEL_RECORDS, dbo.MAINTENANCE_RECORDS, dbo.RECEIPTS,
   dbo.REMINDERS, dbo.ROLES, dbo.SERVICE_TYPES, dbo.USERS,
   dbo.VEHICLES, dbo.VEHICLE_TYPES
   ```

✅ **خلصت قاعدة البيانات!**

---

# 🟪 الجزء الرابع: تثبيت Visual Studio 2022

> (لو منصّب عندك، تخطّى هالجزء)

## الخطوة 4.1 — التحميل

1. روح على: **https://visualstudio.microsoft.com/downloads/**
2. تحت **Visual Studio 2022** → **Community** (مجاني) → **Free download**.

## الخطوة 4.2 — اختيار المكوّنات (الأهم!)

1. شغّل المُثبّت.
2. حط صح على: ☑ **ASP.NET and web development**
3. (موجودة تلقائياً مع هالـ workload: SQL Server Data Tools + LocalDB)
4. اضغط **Install** واستنى 30-60 دقيقة.

---

# 🟧 الجزء الخامس: فتح وتشغيل المشروع

## الخطوة 5.1 — فك ضغط المشروع

1. كليك يمين على **`CarCareTracker.zip`** → **Extract All** لمجلد مثل `C:\Projects\`.

## الخطوة 5.2 — فتح المشروع

1. Double-click على **`CarCareTracker.sln`**.

## الخطوة 5.3 — استعادة مكتبات NuGet

1. **Tools → NuGet Package Manager → Package Manager Console**.
2. اكتب: `Update-Package -reinstall` ثم Enter (دقيقتين).
3. **Build → Rebuild Solution** → لازم يطلع "Build succeeded".

## الخطوة 5.4 — التأكد من الاتصال

افتح **Web.config** وتأكد إن الـ Server name مطابق لإعداداتك:

```xml
Server=.\SQLEXPRESS;Database=CarCareTracker;Integrated Security=True;TrustServerCertificate=True;
```

| لو عندك | استخدم |
|---------|--------|
| SQL Express | `Server=.\SQLEXPRESS;` |
| LocalDB (مع VS) | `Server=(localdb)\MSSQLLocalDB;` |
| Default instance | `Server=localhost;` |

## الخطوة 5.5 — التشغيل! 🚀

اضغط **F5** → يفتح المتصفح على صفحة Login.

## الخطوة 5.6 — أول حساب Admin

1. اضغط **Create new account** وسجّل حساب.
2. في SSMS، نفّذ (بدّل الإيميل):
   ```sql
   USE CarCareTracker;
   UPDATE USERS SET role_id = 1 WHERE email = 'your-email@example.com';
   ```
3. سجّل خروج ودخول → صرت Admin!

---

# 🛠️ حل المشاكل الشائعة

| المشكلة | الحل |
|---------|------|
| `A network-related error / server not found` | تأكد إن اسم الـ Server صح في Web.config. جرّب `.\SQLEXPRESS` أو `(localdb)\MSSQLLocalDB` |
| `Cannot open database CarCareTracker` | ما شغّلت السكربت — افتحه في SSMS واضغط F5 |
| `Login failed for user` | استخدم `Integrated Security=True` (Windows Auth) متل ما هو بالملف |
| `TrustServerCertificate` error | تأكد إن السطر فيه `TrustServerCertificate=True;` |
| References صفراء في VS | Tools → NuGet → Console → `Update-Package -reinstall` |
| الصفحة بيضا بعد Login | الجداول ما اتعملت — شغّل السكربت في SSMS |

---

# 📌 معلومات مهمة احفظها

| المعلومة | القيمة |
|----------|--------|
| اسم قاعدة البيانات | `CarCareTracker` |
| Server name | `.\SQLEXPRESS` (أو حسب تثبيتك) |
| Authentication | Windows Authentication |
| اسم المشروع | CarCareTracker |

---

# ✅ ملخّص الترتيب

```
1. ثبّت SQL Server Express     → احفظ اسم الـ Instance
2. ثبّت SSMS                    → اتصل بالسيرفر
3. شغّل السكربت SQL            → ينشئ قاعدة + 9 جداول
4. ثبّت Visual Studio 2022     → workload: ASP.NET (لو مش منصّب)
5. افتح CarCareTracker.sln     → NuGet Restore + Rebuild
6. تأكد من Web.config          → اسم الـ Server صح
7. اضغط F5                     → صفحة Login
8. سجّل حساب + رقّيه Admin      → جاهز!
```

بالتوفيق يا جواد! 🎓🚗
