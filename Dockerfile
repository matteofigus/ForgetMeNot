FROM thaiphan/mono:3.8.0.9
RUN apt-get update
ADD . /app
WORKDIR /app
RUN xbuild src/ReminderService/ReminderService.sln


