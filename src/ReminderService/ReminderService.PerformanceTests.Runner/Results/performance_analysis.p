#Plot script file for performance of various components in ForgetMeNot

set terminal png size 500,400 enhanced font "Helvetica,10"
set output 'plot.png'
set ylabel 'Count'
set xlabel 'Times (ms)'
plot 'InsertPriorityQueueTest.data' with lines