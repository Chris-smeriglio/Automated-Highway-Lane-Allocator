import numpy as np
import matplotlib.pyplot as plt
import csv
import os

def moving_average(a, n=3) :
    ret = np.cumsum(a, dtype=float)
    ret[n:] = ret[n:] - ret[:-n]
    return ret[n - 1:] / n


with open('L1_0.01R3_10.csv') as csv_file:
    csv_reader = csv.reader(csv_file, delimiter=',')
    line_count = 0
    throughput = []
    counter = 0
    i = 0
    lane = 3
    spawnRate = 0.49
    lane3 = []
    lane2 = []
    lane1 = []
    for row in csv_reader:
        if int(row[1]) <= 60 and counter > 50:

            filtered_throughput = moving_average(throughput, n=80)
            meanTput = np.mean(filtered_throughput[100:320])
            #print("Lane: "+ str(lane) + " Spawn Rate " + str(spawnRate)+ " Tput: " + str(meanTput))
            i+=1
            n = np.arange(len(throughput))
            xlab = n / 50
            movAvg = moving_average(throughput, n=80)
            plt.plot(xlab,throughput)
            plt.title("Spawn Time: " + str(round(spawnRate, 1)) + "    Lanes: " + str(lane))
            plt.xlabel("Time (seconds)")
            plt.ylabel("Throughput")
            n = np.arange(len(movAvg))
            xlab = n / 50
            plt.plot(xlab,movAvg)
            plt.savefig('plots2/' + str(i))
            plt.close()

            if lane == 1:
                lane = 3
                lane3.append([spawnRate,meanTput])
                spawnRate -= .01

            elif lane == 2:
                lane -= 1
                lane2.append([spawnRate,meanTput])

            elif lane == 3:
                lane -= 1
                lane1.append([spawnRate,meanTput])

            line_count += 1
            counter = 0
            throughput.clear()


        elif (row[2] != "Infinity" and row[2] != "NaN"):
            throughput.append(float(row[2]))
            line_count += 1
            counter += 1

        else: line_count += 1
        if (i >= 147 or line_count >= 58947) : break

    with open('data_file5.csv', mode='w') as data_file:
        data_writer = csv.writer(data_file, delimiter=',', quotechar='"', quoting=csv.QUOTE_MINIMAL)


        for lane in lane1:
            data_writer.writerow(lane)
            
        for lane in lane2:
            data_writer.writerow(lane)

        for lane in lane3:
            data_writer.writerow(lane)

    # n = np.arange(len(lane1))
    # rate = n/10
    # plt.plot(rate,lane1)
    # n = np.arange(len(lane2))
    # rate = n / 10
    # plt.plot(rate,lane2)
    # n = np.arange(len(lane3))
    # rate = n / 10
    # plt.plot(rate,lane3)
    # labels = [item.get_text() for item in ax.get_xticklabels()]
    # plt.xticks(np.arange(9),('3.5','3','2.5','2.0','1.5','1.0','0.5','0.0','-0.5'))
    # plt.title("Throughput at increasing spawn rates")
    # plt.ylabel("Mean Throughput")
    # plt.xlabel("Respawn rate")
    # plt.show()
