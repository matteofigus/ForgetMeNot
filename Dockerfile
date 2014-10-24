FROM seif/mono 
RUN apt-get update
RUN apt-get install -y curl mono-devel nuget
WORKDIR /tmp
ENV EnableNuGetPackageRestore true
#this imports some required ssl keys http://stackoverflow.com/questions/10781279/c-sharp-the-authentication-or-decryption-has-failed-error-while-using-twitt
RUN mozroots --import --ask-remove 
ADD . /app
WORKDIR /app
RUN nuget sources Add -Name myget-opentable-dev -Source https://opentable.myget.org/F/dev/ -UserName $MYGET_USERNAME -Password $MYGET_PASSWORD -StorePasswordInClearText
RUN nuget restore src/ReminderService/ReminderService.sln -Source https://opentable.myget.org/F/dev/ -Source https://www.nuget.org/api/v2/
RUN xbuild src/ReminderService/ReminderService.sln
EXPOSE 8080
CMD mono src/ReminderService/ReminderService.Hosting.NancySelf/bin/Debug/ForgetMeNot.SelfHosted.exe
