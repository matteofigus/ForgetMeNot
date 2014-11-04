FROM rmacdonaldsmith/docker-debian-mono-devel
RUN apt-get install -y curl
WORKDIR /tmp
ENV EnableNuGetPackageRestore true
ADD . /app
WORKDIR /app
RUN . /app/auth.sh && nuget sources Add -Name myget-opentable-dev -Source https://opentable.myget.org/F/dev/ -UserName $MYGET_USERNAME -Password $MYGET_PASSWORD -StorePasswordInClearText
RUN nuget restore src/ReminderService/ReminderService.sln -Source https://opentable.myget.org/F/dev/ -Source https://www.nuget.org/api/v2/
RUN xbuild src/ReminderService/ReminderService.sln
EXPOSE 8080
CMD mono src/ReminderService/ReminderService.Hosting.NancySelf/bin/Debug/ForgetMeNot.SelfHosted.exe
