# 1. Base stage - يحتوي على بيئة التشغيل فقط (Runtime) وحجمه صغير
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
# منذ .NET 8 وما بعده، البورت الافتراضي أصبح 8080 بدلاً من 80
EXPOSE 8080

# 2. Build stage - يحتوي على أدوات البناء (SDK) وحجمه كبير
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# نسخ ملفات الـ csproj واستعادة حزم الـ NuGet أولاً (للاستفادة من الكاش الخاص بـ Docker)
COPY ["SweetShop/SweetShop.csproj", "SweetShop/"]
RUN dotnet restore "SweetShop/SweetShop.csproj"

# نسخ باقي ملفات المشروع
COPY . .
WORKDIR "/src/SweetShop"

# بناء المشروع (Build)
RUN dotnet build "SweetShop.csproj" -c Release -o /app/build

# 3. Publish stage - نشر المخرجات بدون ملفات المصدر (Source code)
FROM build AS publish
RUN dotnet publish "SweetShop.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. Final stage - دمج المخرجات من مرحلة Publish داخل صورة الـ Runtime الصافية
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# تحديد ماذا سيحدث عند تشغيل الحاوية
ENTRYPOINT ["dotnet", "SweetShop.dll"]
