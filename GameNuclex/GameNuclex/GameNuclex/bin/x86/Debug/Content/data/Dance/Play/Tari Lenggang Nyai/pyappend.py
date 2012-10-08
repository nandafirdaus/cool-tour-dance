import sys
import io
import os
import glob

files = glob.glob("*.csv")

for ii in files :
	rr = open(ii, "a")
	rr.write("Default\n")
	rr.close()