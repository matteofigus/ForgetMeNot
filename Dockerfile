FROM thaiphan/mono:3.8.0.9
RUN apt-get update
WORKDIR /tmp
RUN curl -O http://artifactory.otenv.com:8081/artifactory/dev_tools/NuGet/NuGet.exe
ENV EnableNuGetPackageRestore true
#this imports some required ssl keys http://stackoverflow.com/questions/10781279/c-sharp-the-authentication-or-decryption-has-failed-error-while-using-twitt
RUN mozroots --import --ask-remove 
ADD . /app
WORKDIR /app
#there's something very strange with an error "Native error= Cannot find the specified file"
#the first run fails, but creates the file used in the second run, which works....???
RUN mono /tmp/NuGet.exe restore src/ReminderService/ReminderService.sln || mono /root/.local/share/NuGet/NuGet.exe restore src/ReminderService/ReminderService.sln
RUN xbuild src/ReminderService/ReminderService.sln
EXPOSE 8080
CMD mono src/ReminderService/ReminderService.Hosting.NancySelf/bin/Debug/ForgetMeNot.SelfHosted.exe
